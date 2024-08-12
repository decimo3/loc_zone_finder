namespace ZoneLocator;
public static partial class Ferramentas
{
  public static Double CalcularDistancia(Double lat1, Double lon1, Double lat2, Double lon2)
  {
    // Aproximadamente 111.32 km por grau
    const double metersPerDegree = 111320;

    var deltaLat = (lat2 - lat1) * metersPerDegree;
    var deltaLon = (lon2 - lon1) * metersPerDegree * Math.Cos(lat1 * Math.PI / 180);

    // Distância Euclidiana (hipotenusa)
    var distance = Math.Sqrt(deltaLat * deltaLat + deltaLon * deltaLon);

    return distance; // Distância em metros
  }
}

