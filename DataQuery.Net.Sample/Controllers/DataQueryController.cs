using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DataQuery.Net.Sample.Models;

namespace DataQuery.Net.Sample.Controllers
{
    public class DataQueryController : Controller
    {
        public IDataQuery _dataQuery;
        public DataQueryController(IDataQuery dataQuery)
        {
            _dataQuery = dataQuery;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] DataQueryFilterParams param)
        {
            var results = await _dataQuery.QueryAsync(param);
            return Ok(results);
        }

        [HttpGet("schema")]
        public IActionResult Schema()
        {
            var results = _dataQuery.GetSchema();
            return Ok(results);
        }

    }
}
