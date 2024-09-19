namespace ZoneLocator;
public static class Localizador
{
  public static void Main(String[] args)
  {
    using(var database = new Database())
    {
      if(args.Contains("--first-run"))
      {
        database.SetTemplates();
        var templates = DataSeed.GetTemplatesFromFile();
        database.AddTemplates(templates);
        return;
      }
      else
      {
        var latitude = Double.Parse(args[0], System.Globalization.CultureInfo.InvariantCulture);
        var longitude = Double.Parse(args[1], System.Globalization.CultureInfo.InvariantCulture);
        var templates = database.GetTemplates(latitude, longitude, 1_000);
        if(!templates.Any())
        {
          Console.WriteLine("404: Não foi encontrado nenhum equipamento próximo a sua localização!");
          System.Environment.Exit(-1);
        }
        if(args.Contains("--json"))
        {
          var json = System.Text.Json.JsonSerializer.Serialize(templates);
          Console.WriteLine(json);
          System.Environment.Exit(0);
        }
        Console.WriteLine(new String('#', 40));
        foreach (var template in templates)
        {
          Console.WriteLine(template.ToString());
          Console.WriteLine(new String('#', 40));
        }
      }
    }
  }
}
