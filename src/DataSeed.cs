using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.XPath;
namespace ZoneLocator;
public static class DataSeed
{
  public static List<Template> GetTemplatesFromFile()
  {
    var templates = new List<Template>();
    var current = System.IO.Directory.GetCurrentDirectory();
    var folder = System.IO.Path.Combine(current, "gpx");
    var files = System.IO.Directory.GetFiles(folder);
    foreach (var file in files)
    {
      var xml = System.Xml.Linq.XDocument.Load(file);
      if(xml.Root is null) throw new NullReferenceException("O elemento 'root' é nulo e inválido!");
      var nodes = xml.Root.Elements();
      foreach (var node in nodes)
      {
        if(node.Name.LocalName != "wpt") continue;
        var template = new Template();
        var attributes = node.Attributes();
        foreach (var attribute in attributes)
        {
          if(attribute.Name.LocalName == "lat") template.Lat = Double.Parse(attribute.Value, System.Globalization.CultureInfo.InvariantCulture);
          else if(attribute.Name.LocalName == "lon") template.Lon = Double.Parse(attribute.Value, System.Globalization.CultureInfo.InvariantCulture);
          else throw new InvalidOperationException("Não foi possível coletar as coordenadas!");
        }
        var elements = node.Elements();
        foreach (var element in elements)
        {
          if(element.Name.LocalName == "name") template.Nome = element.Value;
          if(element.Name.LocalName == "type") template.Tipo = element.Value;
        }
        templates.Add(template);
      }
    }
    return templates;
  }
}
