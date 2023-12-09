using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;

namespace ExpressTools.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class FitRadiatorCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            Reference pickedElementRef = uiDoc.Selection.PickObject(ObjectType.Element);
            Element pickedElement = doc.GetElement(pickedElementRef);
            Parameter radiatorWidthParam;
            double relatedWindowWidth;
            try
            {
                if (pickedElement is FamilyInstance familyInstance == false)
                {
                    throw new InvalidOperationException("Invalid Radiator element selected.");
                }

                radiatorWidthParam = RadiatorService.GetRadiatorWidthParameter(familyInstance);
                relatedWindowWidth = RadiatorService.FindTopWindowWidth(doc, pickedElement);

            }
            catch (Exception ex)
            {
                TaskDialog.Show("Invalid Operation",ex.Message);
                return Result.Failed;
            }

             

            // Start a transaction to commit the changes
            using (Transaction transaction = new Transaction(doc, "Set Width Parameter"))
            {
                transaction.Start();
                radiatorWidthParam.Set(relatedWindowWidth);
                transaction.Commit();
            }

            return Result.Succeeded;
        }


    }
}
