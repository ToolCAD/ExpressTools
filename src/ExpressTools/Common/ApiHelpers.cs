using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

public static class ApiHelpers
{
    public static XYZ FindCenterPoint(XYZ firstPoint, XYZ secondPoint, LocationPoint elementLocation)
    {
        return new XYZ((firstPoint.X + secondPoint.X) / 2, (firstPoint.Y + secondPoint.Y) / 2, elementLocation.Point.Z);
    }
    public static double GetDistanceBetweenElements(Element element1, Element element2)
    {
        var box1 = element1.get_BoundingBox(null);
        var box2 = element2.get_BoundingBox(null);

        var fromPoint = (box1.Min + box1.Max) * 0.5;
        var toPoint = (box2.Min + box2.Max) * 0.5;

        return fromPoint.DistanceTo(toPoint);

    }

    public static Element FindNearestElement(Element selectedElement, List<Element> otherElements)
    {

        double minDistance = double.MaxValue;
        Element result = null;

        foreach (var window in otherElements)
        {
            double distance = GetDistanceBetweenElements(selectedElement, window);

            if (distance < minDistance)
            {
                minDistance = distance;
                result = window;
            }
        }

        return result;
    }

    public static List<Element> FindAllWindowsInstances(Document doc)
    {
        var windowsCollector = new FilteredElementCollector(doc);
        var windows = windowsCollector
            .OfCategory(BuiltInCategory.OST_Windows)
            .OfClass(typeof(FamilyInstance));
        return windows.ToList();
    }

    public static List<Element> FindSimilarInstances(Document doc, Element element)
    {
        var familyInstance = element as FamilyInstance;

        if (familyInstance != null)
        {
            FamilySymbol familySymbol = familyInstance.Symbol;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var similarInstances = collector
                .OfClass(typeof(FamilyInstance))
                .Where(x => ((x as FamilyInstance).Symbol != null && (x as FamilyInstance).Symbol.Name == familySymbol.Name));

            return similarInstances.ToList();
        }

        return new List<Element>();
    }
}