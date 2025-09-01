using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

public class FilterController : Controller
{
    private readonly IConfiguration _config;

    public FilterController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public IActionResult GetBolumler()
    {
        var result = new List<object>();

        using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        using (SqlCommand cmd = new SqlCommand("SP_GET_BOLUMLER", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    result.Add(new
                    {
                        Id = rdr["BOLUM_ID"],
                        Ad = rdr["BOLUM_ADI"]
                    });
                }
            }
        }

        return Json(result);
    }

    [HttpGet]
    public IActionResult GetKurumlar(int bolumId)
    {
        var result = new List<object>();

        using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        using (SqlCommand cmd = new SqlCommand("SP_GET_KURUMLAR", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BOLUM_ID", bolumId);
            con.Open();

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    result.Add(new
                    {
                        Id = rdr["KURUM_ID"],
                        Ad = rdr["KURUM_ADI"]
                    });
                }
            }
        }

        return Json(result);
    }

    [HttpGet]
    public IActionResult GetBirimler(int kurumId)
    {
        var result = new List<object>();

        using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        using (SqlCommand cmd = new SqlCommand("SP_GET_BIRIMLER", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@KURUM_ID", kurumId);
            con.Open();

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    result.Add(new
                    {
                        Id = rdr["BIRIM_ID"],
                        Ad = rdr["BIRIM_ADI"]
                    });
                }
            }
        }

        return Json(result);
    }
}
