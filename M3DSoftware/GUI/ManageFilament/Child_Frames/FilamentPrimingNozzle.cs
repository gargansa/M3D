﻿using M3D.Graphics;
using M3D.Graphics.Widgets2D;
using M3D.GUI.Controller;
using M3D.Spooling.Client;
using M3D.Spooling.Common;
using M3D.Spooling.Common.Utils;

namespace M3D.GUI.ManageFilament.Child_Frames
{
  internal class FilamentPrimingNozzle : Manage3DInkChildWindow
  {
    private TextWidget text_main;

    public FilamentPrimingNozzle(int ID, GUIHost host, Manage3DInkMainWindow mainWindow)
      : base(ID, host, mainWindow)
    {
    }

    public override void MyButtonCallback(ButtonWidget button)
    {
    }

    public override void Init()
    {
      CreateManageFilamentFrame("Remove 3D Ink", "Please wait. \n\nRemove filament is extruding a small amount of filament first to prevent clogs.", false, false, false, false, false, false);
      text_main = (TextWidget)FindChildElement(3);
    }

    public override void OnActivate(Mangage3DInkStageDetails details)
    {
      base.OnActivate(details);
      text_main.Text = "";
      text_main.Text = CurrentDetails.current_spool.filament_type != FilamentSpool.TypeEnum.CAM ? "Please wait. \n\nRemove filament is extruding a small amount of filament first to prevent clogs." : "Please wait. \n\nRemove filament is extruding a small amount of filament first to prevent clogs.\n\nWarning: Chameleon Ink may appear white when heated and exiting nozzle.";
      PrinterObject selectedPrinter = MainWindow.GetSelectedPrinter();
      if (selectedPrinter == null)
      {
        return;
      }

      if (CurrentDetails.current_spool == null)
      {
        MainWindow.ResetToStartup();
      }
      else
      {
        var num = (int) selectedPrinter.SendManualGCode(new AsyncCallback(MainWindow.GotoPageAfterOperation), new Manage3DInkMainWindow.PageAfterLockDetails(selectedPrinter, Manage3DInkMainWindow.PageID.Page2_RetractingFilament, CurrentDetails), "G4 S5", "G91", PrinterCompatibleString.Format("G0 E{0} F{1}", 50f, 90.0));
      }
    }
  }
}
