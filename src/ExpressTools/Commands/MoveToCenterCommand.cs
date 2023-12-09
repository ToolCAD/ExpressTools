using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Diagnostics;

namespace ExpressTools.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class MoveToCenterCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Access the active Revit document
                var uiDoc = commandData.Application.ActiveUIDocument;
                var doc = uiDoc.Document;

                Element pickedElement = null;

                try
                {
                    // Prompt user to select an element
                    var pickedElementRef = uiDoc.Selection.PickObject(ObjectType.Element);
                    pickedElement = doc.GetElement(pickedElementRef);
                }
                catch (Exception ex)
                {

                    Debug.WriteLine(ex.Message);
                    return Result.Failed;
                }


                if (pickedElement == null)
                {
                    Debug.WriteLine("No element selected");
                    return Result.Failed;
                }

                // Prompt user to select two points
                XYZ firstPoint = null;
                XYZ secondPoint = null;

                try
                {
                    firstPoint = uiDoc.Selection.PickPoint("Select first point");
                    secondPoint = uiDoc.Selection.PickPoint("Select second point");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return Result.Failed;
                }

                // Get the element's location
                var elementLocation = pickedElement.Location as LocationPoint;

                var isValid = ValidatePoints(firstPoint, secondPoint, elementLocation);
                if (!isValid)
                {
                    Debug.WriteLine("Point not found");
                    return Result.Failed;
                }

                XYZ centerPoint = ApiHelpers.FindCenterPoint(firstPoint, secondPoint, elementLocation);

                MoveToCenterTransaction(doc, elementLocation, centerPoint);

                Debug.WriteLine("Element moved to the center between the selected points.");
                return Result.Succeeded;

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                // User canceled the operation
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                message = ex.Message;
                Debug.WriteLine(message);
                return Result.Failed;
            }
        }

        private static bool ValidatePoints(XYZ firstPoint, XYZ secondPoint, LocationPoint elementLocation)
        {
            var isValid = true;
            if (firstPoint == null || secondPoint == null || elementLocation == null)
            {
                isValid = false;
            }

            return isValid;
        }

        private static void MoveToCenterTransaction(Document doc, LocationPoint elementLocation, XYZ centerPoint)
        {
            // Move the element to the calculated center point
            Transaction transaction = new Transaction(doc, "Move Element to Center");
            transaction.Start();
            elementLocation.Point = centerPoint;
            transaction.Commit();
        }

    }
}
