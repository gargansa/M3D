﻿using M3D.GUI.Controller;
using M3D.GUI.Controller.Settings;
using M3D.Model;
using M3D.Spooling.Common;
using System.Collections.Generic;

namespace M3D.GUI.Views.Printer_View.Print_Dialog_Widget
{
  internal class PrintJobDetails
  {
    public List<ModelTransform> slicer_objects;
    public List<PrintDetails.ObjectDetails> objectDetailsList;
    public M3D.Slicer.General.PrintSettings settings;
    public JobOptions jobOptions;
    public PrinterObject printer;
    public string preview_image;
    public float Estimated_Print_Time;
    public float Estimated_Filament;
    public bool auto_untethered_print;
    public bool sdSaveOnly_print;
    public bool print_to_file;
    public string printToFileOutputFile;
    public bool autoPrint;

    public PrintJobDetails()
    {
      slicer_objects = new List<ModelTransform>();
      objectDetailsList = new List<PrintDetails.ObjectDetails>();
    }

    public void GenerateSlicerSettings(PrinterObject printer, PrinterView printerview)
    {
      settings = new M3D.Slicer.General.PrintSettings();
      settings.SetPrintDefaults();
      if (printer != null)
      {
        FilamentSpool currentFilament = printer.GetCurrentFilament();
        settings.filament_info = currentFilament;
        if (currentFilament.filament_type == FilamentSpool.TypeEnum.HIPS)
        {
          settings.filament_info.filament_type = FilamentSpool.TypeEnum.ABS;
        }
      }
      settings.transformation = printerview.GetObjectSlicerTransformation();
    }
  }
}
