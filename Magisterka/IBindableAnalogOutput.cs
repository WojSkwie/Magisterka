﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public enum EAnalogOutput
    {
        temperature = 0,
        humidity = 1,


    }

    public interface IBindableAnalogOutput 
    {
        void InitializeParametersList();
        List<BindableAnalogOutputPort> BindedInputs { get; set; }
        List<EAnalogOutput> GetListOfParams();
        int GetParamter(EAnalogOutput analogOutput);
    }
}
