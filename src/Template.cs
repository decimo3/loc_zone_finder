namespace ZoneLocator;
public class Template
{
  public String Nome { get; set; } = String.Empty;
  public String Tipo { get; set; } = String.Empty;
  public Double Lat { get; set; } = 0;
  public Double Lon { get; set; } = 0;
  public Double Mts { get; set; } = 0;
  public override string ToString()
  {
    return $"Equipamento: {this.Nome}\n" +
           $"Coordenadas: {this.Lat}.{this.Lon}\n" +
           $"Distância: {Math.Round(this.Mts)}mts";
  }
}
