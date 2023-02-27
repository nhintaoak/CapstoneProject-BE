﻿using AutoMapper;
using CapstoneProject_BE.AutoMapper;
using CapstoneProject_BE.DTO;
using CapstoneProject_BE.Helper;
using CapstoneProject_BE.Models;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProject_BE.Controllers.Import
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly InventoryManagementContext _context;
        public IMapper mapper;
        public ImportController(InventoryManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            mapper = config.CreateMapper();
        }
        [HttpPut("UpdateImportOrder")]
        public async Task<IActionResult> UpdateImportOrder(ImportOrderDTO p)
        {
            try
            {
                if (p != null)
                {
                    var dbimport = _context.ImportOrders.Include(a => a.ImportOrderDetails).SingleOrDefault(a => a.ImportId == p.ImportId);
                    var result = mapper.Map<ImportOrder>(p);
                    if (dbimport.State == 0)
                    {
                        _context.RemoveRange(dbimport.ImportOrderDetails);
                        await _context.SaveChangesAsync();
                        _context.ChangeTracker.Clear();
                        _context.Update(result);
                        await _context.SaveChangesAsync();
                        return Ok("Thành công");
                    }
                    else
                    {
                        return BadRequest("Không thể chỉnh sửa phiếu này");
                    }
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpPut("a")]
        public async Task<IActionResult> a(int b)
        {
            try
            {
                        var dbimport = _context.ImportOrders.Include(a=>a.ImportOrderDetails).SingleOrDefault(a=>a.ImportId==b);
                return Ok(dbimport);
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpPost("CreateImportOrder")]
        public async Task<IActionResult> CreateImportOrder(ImportOrderDTO p)
        {
            try
            {
                if (p != null)
                {
                    var result = mapper.Map<Models.ImportOrder>(p);
                    result.Created = DateTime.Now;
                    result.State = 0;
                    result.ImportCode = "NAHA"+TokenHelper.GenerateNumericToken(16);
                    _context.Add(result);
                    await _context.SaveChangesAsync();
                    return Ok("Thành công");
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpPost("ApproveImport")]
        public async Task<IActionResult> ApproveImport(int importid)
        {
            try
            {
                var result = await _context.ImportOrders.SingleOrDefaultAsync(x => x.ImportId == importid);
                if (result != null&&result.State==0)
                {
                    result.State = 1;
                    result.Approved = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return Ok("Thành công");
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpGet("GetImportOrder")]
        public async Task<IActionResult> GetImport(int offset, int limit, int? supId = 0, int? state = -1, string? code = "")
        {
            try
            {
                var result = await _context.ImportOrders.Include(a=>a.Supplier)
                    .Where(x => (x.Supplier.SupplierName.Contains(code)||x.ImportCode.Contains(code)||code=="")
                && (x.SupplierId == supId || supId == 0) && (x.State == state || state == -1)
                 ).ToListAsync();
                if (limit > result.Count() && offset >= 0)
                {
                    return Ok(new ResponseData<ImportOrder>
                    {
                        Data = result.Skip(offset).Take(result.Count()).ToList(),
                        Offset = offset,
                        Limit = limit,
                        Total = result.Count()
                    });
                }
                else if (offset >= 0)
                {
                    return Ok(new ResponseData<ImportOrder>
                    {
                        Data = result.Skip(offset).Take(limit).ToList(),
                        Offset = offset,
                        Limit = limit,
                        Total = result.Count()
                    });
                }
                else
                {
                    return NotFound("Không kết quả");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpGet("GetImportDetail")]
        public async Task<IActionResult> GetImportDetail(int importid)
        {
            try
            {
                var result = await _context.ImportOrders
                    .Include(x=>x.ImportOrderDetails).ThenInclude(x=>x.Product).ThenInclude(x=>x.MeasuredUnits).Include(x=>x.Supplier).Include(x=>x.User)
                    .SingleOrDefaultAsync(x => x.ImportId == importid);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpPost("DenyImport")]
        public async Task<IActionResult> DenyImport(int importid)
        {
            try
            {
                var result = await _context.ImportOrders.SingleOrDefaultAsync(x => x.ImportId == importid);
                if (result != null&&result.State==0)
                {
                    result.State = 3;
                    result.Approved = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return Ok("Thành công");
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpPost("Import")]
        public async Task<IActionResult> Import(int importid)
        {
            try
            {
                var result = await _context.ImportOrders.Include(a => a.ImportOrderDetails).SingleOrDefaultAsync(x => x.ImportId == importid);
                if (result != null && result.State == 1)
                {
                    result.State = 2;
                    result.Completed = DateTime.Now;
                    foreach (var detail in result.ImportOrderDetails)
                    {
                        var product = await _context.Products.SingleOrDefaultAsync(x => x.ProductId == detail.ProductId);
                        var history = new ProductHistory
                        {
                            ProductId = product.ProductId,
                            ActionId = 1
                        };
                        int total = 0;
                        if (detail.MeasuredUnitId != null)
                        {
                            detail.MeasuredUnit = await _context.MeasuredUnits.SingleOrDefaultAsync(x => x.MeasuredUnitId == detail.MeasuredUnitId);
                            total = detail.Amount * detail.MeasuredUnit.MeasuredUnitValue;
                            product.InStock += total;
                        }
                        else
                        {
                            total = detail.Amount;
                            history.AmountDifferential = $"+{detail.Amount}";
                            product.InStock += total;
                        }
                        history.AmountDifferential = $"+{total}";
                        history.CostPrice = product.CostPrice;
                        product.CostPrice = (detail.Amount*detail.CostPrice + product.InStock * product.CostPrice) / (total + product.InStock);
                        var costdifferential = product.CostPrice - history.CostPrice;
                        history.CostPriceDifferential = costdifferential > 0 ? $"+{costdifferential}" : $"-{costdifferential}";
                        history.Amount = product.InStock;
                        history.Date = DateTime.Now;
                        _context.Add(history);
                    }
                    await _context.SaveChangesAsync();
                    return Ok("Thành công");
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
