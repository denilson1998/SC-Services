using System.IO;
using SharpKml.Base;
using SharpKml.Dom;

namespace Delivery.Domain.Utils;
public class KmlUtils
{
    public Placemark ParseKml()
    {
        FileStream fsSource = new FileStream("/test.kml",
            FileMode.Open, FileAccess.Read);
        Parser parser = new Parser();
        parser.Parse(fsSource);
        var placemark = (Placemark)parser.Root;
        var a = placemark.Geometry.Children;
        return placemark;
    }
}