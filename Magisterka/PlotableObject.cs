﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public abstract class PlotableObject
    {
        public List<PlotData> PlotDataList { get; set; }

        public PlotableObject()
        {
            PlotDataList = new List<PlotData>();
        }

        public PlotData GetPlotData(EDataType dataType)
        {
            if (!PlotDataList.Any(item => item.DataType == dataType)) return null;
            PlotData plotData = PlotDataList.First(item => item.DataType == dataType);
            return plotData;
        }

        public List<PlotData> GetAllPlotData()
        {
            return PlotDataList;
        }


    }
}