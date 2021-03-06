﻿using M3D.Spooling.Common.Utils;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace M3D.Spooling.Printer_Profiles
{
  public class SupportedFeaturesProfile
  {
    private WriteOnce<Dictionary<string, int>> m_supportedFeatureMapping;

    public SupportedFeaturesProfile(Dictionary<string, int> mapping)
      : this()
    {
      m_supportedFeatureMapping.Value = new Dictionary<string, int>(mapping);
    }

    public SupportedFeaturesProfile()
    {
      m_supportedFeatureMapping = new WriteOnce<Dictionary<string, int>>(null);
    }

    public SupportedFeaturesProfile(SupportedFeaturesProfile other)
    {
      m_supportedFeatureMapping = new WriteOnce<Dictionary<string, int>>();
      if (other.m_supportedFeatureMapping.Value == null)
      {
        return;
      }

      m_supportedFeatureMapping.Value = new Dictionary<string, int>(other.m_supportedFeatureMapping.Value);
    }

    public int GetFeatureSlot(string featureName)
    {
      if (m_supportedFeatureMapping.Value.ContainsKey(featureName))
      {
        return m_supportedFeatureMapping.Value[featureName];
      }

      return -1;
    }

    public IEnumerable<KeyValuePair<string, int>> EnumerateList()
    {
      return SupportedFeatureMapping;
    }

    [XmlIgnore]
    public bool HasSupportedFeatures
    {
      get
      {
        if (m_supportedFeatureMapping.Value != null)
        {
          return m_supportedFeatureMapping.Value.Count > 0;
        }

        return false;
      }
    }

    public SerializableDictionary<string, int> SupportedFeatureMapping
    {
      get
      {
        if (m_supportedFeatureMapping.Value != null)
        {
          return new SerializableDictionary<string, int>(m_supportedFeatureMapping.Value);
        }

        return null;
      }
      set
      {
        try
        {
          m_supportedFeatureMapping.Value = value;
        }
        catch (InvalidOperationException ex)
        {
          throw new InvalidOperationException("SupportedFeatureMapping can not be modified after creation.", ex);
        }
      }
    }
  }
}
