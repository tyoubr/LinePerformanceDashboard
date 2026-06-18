using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LinePerformanceDashboard.Controllers
{
    public class LinePerformanceDashboardController : Controller
    {
        private readonly IConfiguration _configuration;

        public LinePerformanceDashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardCardData()
        {
            var data = new List<LineWiseDTO>();

            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("56_LINE_D", con)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            await con.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data.Add(new LineWiseDTO
                {
                    Date = reader["PROD_DATE"] != DBNull.Value? Convert.ToDateTime(reader["PROD_DATE"]).Date: (DateTime?)null,
                    LineName = reader["LINE_NAME"]?.ToString(),
                    ItemName = reader["ITEM_NAME"]?.ToString(),
                    BuyerName = reader["BUYER_NAME"]?.ToString(),
                    StyleRefNo = reader["STYLE_REF_NO"]?.ToString(),
                    AvgSmv = Convert.ToDecimal(reader["AVG_SMV"] ?? 0),
                    ManPower = Convert.ToDecimal(reader["MAN_POWER"] ?? 0),
                });
            }

            return Ok(data.FirstOrDefault());
        }



        [HttpGet]
        public async Task<IActionResult> GetLineWiseProduction()
        {
            var list = new List<LineWiseProdDTO>();

            var connStr = _configuration.GetConnectionString("DefaultConnection");

            using var con = new SqlConnection(connStr);
            using var cmd = new SqlCommand("56_LINE_D", con)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            await con.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new LineWiseProdDTO
                {
                    TARGET_PER_HOUR = reader["TARGET_PER_HOUR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TARGET_PER_HOUR"]),
                    PROD_DATE = reader["PROD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["PROD_DATE"]),

                    AvgSmv = reader["AVG_SMV"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AVG_SMV"]),
                    ManPower = reader["MAN_POWER"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["MAN_POWER"]),

                    ONE = reader["ONE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ONE"]),
                    TWO = reader["TWO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TWO"]),
                    THREE = reader["THREE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["THREE"]),
                    FOUR = reader["FOUR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["FOUR"]),
                    FIVE = reader["FIVE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["FIVE"]),
                    SIX = reader["SIX"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["SIX"]),
                    SEVEN = reader["SEVEN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["SEVEN"]),
                    EIGHT = reader["EIGHT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["EIGHT"]),
                    NINE = reader["NINE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NINE"]),
                    TEN = reader["TEN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TEN"]),
                    ELEVEN = reader["ELEVEN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ELEVEN"]),
                });
            }

            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetOperatorDetails()
        {
            var list = new List<object>();

            var today = DateTime.Now.Date;

            using (var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string query = @"
            SELECT 
                NAME,
                PROCESS_NAME,
                AVG_CYCLE,
                CAPACITY_HR,
                PROD_DATE,
                REMARK
            FROM TBL_OPERATOR_DETAIL
            WHERE PROD_DATE >= @StartDate
              AND PROD_DATE < DATEADD(DAY, 1, @StartDate)
        ";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StartDate", today);

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var name = rdr["NAME"]?.ToString();
                            var process = rdr["PROCESS_NAME"]?.ToString();
                            var cycle = rdr["AVG_CYCLE"];
                            var cap = rdr["CAPACITY_HR"];
                            var date = rdr["PROD_DATE"];
                            var remark = rdr["REMARK"]?.ToString();

                            // Debug Log
                            System.Diagnostics.Debug.WriteLine(
                                $"NAME={name}, PROCESS={process}, CYCLE={cycle}, CAP={cap}, DATE={date}, REMARK={remark}"
                            );

                            list.Add(new
                            {
                                name = name,
                                process_NAME = process,
                                avg_CYCLE = cycle,
                                capacity_HR = cap,
                                prod_DATE = date,
                                remark = remark
                            });
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"TOTAL ROWS: {list.Count}");

            return Ok(list);
        }
        //        [HttpGet]
        //        public IActionResult GetTopSkilledOperators()
        //        {
        //            var list = new List<object>();

        //            using (var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //            {
        //                con.Open();

        //                string query = @"
        //            SELECT
        //    NAME,
        //    PROCESS_NAME,
        //    CAST(AVG_CYCLE AS FLOAT) AS AVG_CYCLE,
        //    CAST(CAPACITY_HR AS FLOAT) AS CAPACITY_HR,
        //    (CAST(CAPACITY_HR AS FLOAT) / NULLIF(CAST(AVG_CYCLE AS FLOAT), 0)) AS SKILL_SCORE
        //FROM TBL_OPERATOR_DETAIL
        //WHERE PROD_DATE >= CAST(GETDATE() AS DATE)
        //  AND PROD_DATE < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
        //ORDER BY SKILL_SCORE DESC
        //        ";

        //                using (var cmd = new SqlCommand(query, con))
        //                using (var rdr = cmd.ExecuteReader())
        //                {
        //                    while (rdr.Read())
        //                    {
        //                        list.Add(new
        //                        {
        //                            name = rdr["NAME"].ToString(),
        //                            process = rdr["PROCESS_NAME"].ToString(),
        //                            avgCycle = rdr["AVG_CYCLE"],
        //                            capacity = rdr["CAPACITY_HR"],
        //                            score = rdr["SKILL_SCORE"]
        //                        });
        //                    }
        //                }
        //            }

        //            return Ok(list);
        //        }

        public class LineWiseDTO
        {
            public DateTime?Date { get; set; }
            public string? LineName { get; set; }
            public string? ItemName { get; set; }
            public string? BuyerName { get; set; }
            public string? StyleRefNo { get; set; }
            public decimal AvgSmv { get; set; }
            public decimal ManPower { get; set; }
        }

        public class LineWiseProdDTO
        {
            public decimal? TARGET_PER_HOUR { get; set; }
            public decimal AvgSmv { get; set; }
            public decimal ManPower { get; set; }
            public DateTime? PROD_DATE { get; set; }
            public decimal? ONE { get; set; }
            public decimal? TWO { get; set; }
            public decimal? THREE { get; set; }
            public decimal? FOUR { get; set; }
            public decimal? FIVE { get; set; }
            public decimal? SIX { get; set; }
            public decimal? SEVEN { get; set; }
            public decimal? EIGHT { get; set; }
            public decimal? NINE { get; set; }
            public decimal? TEN { get; set; }
            public decimal? ELEVEN { get; set; }

        }
    }
}
