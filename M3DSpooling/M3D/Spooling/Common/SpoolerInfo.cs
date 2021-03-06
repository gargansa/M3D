﻿using M3D.Spooling.Core;
using M3D.Spooling.Printer_Profiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace M3D.Spooling.Common
{
  public class SpoolerInfo
  {
    private static XmlWriterSettings settings = new XmlWriterSettings();
    private static XmlSerializer __class_serializer = null;
    private XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
    [XmlElement("VersionNumber")]
    public VersionNumber Version;
    [XmlElement("PrinterProfileSummary", Type = typeof (EmbeddedFirmwareSummary))]
    public List<EmbeddedFirmwareSummary> SupportPrinterProfiles;
    [XmlElement("PrinterProfile", Type = typeof (PrinterProfile))]
    public List<PrinterProfile> PrinterProfileList;

    public SpoolerInfo()
    {
      SupportPrinterProfiles = new List<EmbeddedFirmwareSummary>();
      PrinterProfileList = new List<PrinterProfile>();
    }

    public SpoolerInfo(SpoolerInfo other)
    {
      CopyFrom(other);
    }

    public SpoolerInfo(string serialization)
      : this()
    {
      Deserialize(serialization);
    }

    public void Deserialize(string serialization)
    {
      using (var textReader = (TextReader) new StringReader(serialization))
      {
        try
        {
          CopyFrom((SpoolerInfo) SpoolerInfo.ClassSerializer.Deserialize(textReader));
        }
        catch (Exception ex)
        {
          ErrorLogger.LogErrorMsg("Loading XML Exception: " + ex.Message + (ex.InnerException != null ? "\nInner Exception: " + ex.InnerException.Message : ""));
          CopyFrom(new SpoolerInfo());
        }
      }
    }

    public string Serialize()
    {
      SpoolerInfo.settings.OmitXmlDeclaration = true;
      var stringWriter = new StringWriter();
      var xmlWriter = XmlWriter.Create(stringWriter, SpoolerInfo.settings);
      ns.Add("", "");
      try
      {
        SpoolerInfo.ClassSerializer.Serialize(xmlWriter, this, ns);
      }
      catch (Exception ex)
      {
        if (Debugger.IsAttached)
        {
          Debugger.Break();
        }

        throw;
      }
      return stringWriter.ToString();
    }

    public void CopyFrom(SpoolerInfo other)
    {
      Version = other.Version;
      SupportPrinterProfiles = new List<EmbeddedFirmwareSummary>(other.SupportPrinterProfiles);
      PrinterProfileList = new List<PrinterProfile>(other.PrinterProfileList);
    }

    public EmbeddedFirmwareSummary GetProfileByName(string name)
    {
      return SupportPrinterProfiles.Find(x => x.Name == name);
    }

    private static XmlSerializer ClassSerializer
    {
      get
      {
        if (SpoolerInfo.__class_serializer == null)
        {
          SpoolerInfo.__class_serializer = new XmlSerializer(typeof (SpoolerInfo));
        }

        return SpoolerInfo.__class_serializer;
      }
    }
  }
}
