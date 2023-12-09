using Autodesk.Revit.DB;
using System;

public static class RadiatorService
{
    public static double FindTopWindowWidth(Document doc, Element selectedRadiator)
    {
        var radiators = ApiHelpers.FindSimilarInstances(doc, selectedRadiator);
        var windows = ApiHelpers.FindAllWindowsInstances(doc);
        if (windows.Count < 1)
        {
            throw new InvalidOperationException("Could not find any windows instances.");
        }

        Element nearestWindow = ApiHelpers.FindNearestElement(selectedRadiator, windows);

        if (nearestWindow == null)
        {
            throw new InvalidOperationException("Could not find any respective windows.");
        }

        Element nearestRadiator = ApiHelpers.FindNearestElement(nearestWindow, radiators);

        if (nearestRadiator.UniqueId!=selectedRadiator.UniqueId)
        {
            throw new InvalidOperationException("No matching window found.");
        }

        double relatedWindowWidth = GetWindowWidth(nearestWindow);

        if (relatedWindowWidth <= 0)
        {
            throw new InvalidOperationException("Invalid windows width found.");
        }
        return relatedWindowWidth;
    }

    private static double GetWindowWidth(Element windowElement)
    {
        double relatedWindowWidth = 0;

        var window = windowElement as FamilyInstance;

        var widthP = window.get_Parameter(BuiltInParameter.DOOR_WIDTH);
        if (widthP != null && widthP.StorageType == StorageType.Double)
        {
            relatedWindowWidth = widthP.AsDouble();
        }

        return relatedWindowWidth;
    }

    public static Parameter GetRadiatorWidthParameter(FamilyInstance familyInstance)
    {
        int parameterId = 18749970; // familyInstance.LookupParameter("Länge");

        ParameterSet parameters = familyInstance.Parameters;

        // Look for the parameter with the specified ID
        Parameter widthParam = null;

        foreach (Parameter param in parameters)
        {
            if (param.Id.Value == parameterId)
            {
                widthParam = param;
                break;
            }
        }

        if (widthParam == null || widthParam.StorageType != StorageType.Double)
        {
            throw new InvalidOperationException("Invalid Radiator element selected.");
        }

        return widthParam;

    }
}
