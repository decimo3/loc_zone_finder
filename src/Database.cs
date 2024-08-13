using System.Data.SQLite;
namespace ZoneLocator;
public class Database : IDisposable
{
  private readonly String connectionString = "Data Source=gps.db";
  private readonly SQLiteConnection connection;
  private readonly SQLiteCommand command;

  public Database()
  {
    this.connection = new SQLiteConnection(connectionString);
    connection.Open();
    this.command = connection.CreateCommand();
  }

  public void SetTemplates()
  {
    this.command.CommandText = @"
      CREATE TABLE IF NOT EXISTS templates (
        nome TEXT NOT NULL,
        tipo TEXT NOT NULL,
        lat DOUBLE NOT NULL,
        lon DOUBLE NOT NULL
      )";
    this.command.ExecuteNonQuery();
  }

  public void AddTemplates(List<Template> templates)
  {
    using(var transaction = this.connection.BeginTransaction())
    {
      this.command.CommandText = @"INSERT INTO templates (nome, tipo, lat, lon)
                                  VALUES (@param1, @param2, @param3, @param4)";
      this.command.Parameters.Add(new SQLiteParameter("@param1"));
      this.command.Parameters.Add(new SQLiteParameter("@param2"));
      this.command.Parameters.Add(new SQLiteParameter("@param3"));
      this.command.Parameters.Add(new SQLiteParameter("@param4"));
      foreach (var template in templates)
      {
        this.command.Parameters["@param1"].Value = template.Nome;
        this.command.Parameters["@param2"].Value = template.Tipo;
        this.command.Parameters["@param3"].Value = template.Lat;
        this.command.Parameters["@param4"].Value = template.Lon;
        this.command.ExecuteNonQuery();
      }
      transaction.Commit();
    }
  }

  public List<Template> GetTemplates(Double lat, Double lon, Int32 raio)
  {
    const double metersPerDegree = 111320; // Aproximadamente 111.32 km (111320 metros) por grau
    var templates = new List<Template>();

    // Convertendo o raio de metros para graus aproximadamente
    var lat_variation = raio / metersPerDegree;
    var lon_variation = raio / (metersPerDegree * Math.Cos(lat * Math.PI / 180));

    var lat_max = lat + lat_variation;
    var lat_min = lat - lat_variation;
    var lon_max = lon + lon_variation;
    var lon_min = lon - lon_variation;

    this.command.CommandText =
        @"SELECT nome, tipo, lat, lon FROM templates
        WHERE lat >= @lat_min AND lat <= @lat_max
        AND lon >= @lon_min AND lon <= @lon_max
        AND tipo = 'ZNA' LIMIT 5";

    this.command.Parameters.AddWithValue("@lat_min", lat_min);
    this.command.Parameters.AddWithValue("@lat_max", lat_max);
    this.command.Parameters.AddWithValue("@lon_min", lon_min);
    this.command.Parameters.AddWithValue("@lon_max", lon_max);

    using(var reader = command.ExecuteReader())
    {
      if(!reader.HasRows) return templates;
      while(reader.Read())
      {
        templates.Add(new Template() {
          Nome = (String)reader["nome"],
          Tipo = (String)reader["tipo"],
          Lat = (Double)reader["lat"],
          Lon = (Double)reader["lon"],
          Mts = Ferramentas.CalcularDistancia(lat, lon, (Double)reader["lat"], (Double)reader["lon"])
        });
      }
    }
    return templates.OrderBy(t => t.Mts).ToList();
  }

  public void Dispose()
  {
    this.command.Dispose();
    this.connection.Close();
    this.connection.Dispose();
  }
}

