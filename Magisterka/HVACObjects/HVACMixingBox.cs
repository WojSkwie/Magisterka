﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public sealed class HVACMixingBox : HVACObject, IBindableAnalogInput
    {
        private HVACMixingBox CoupledMixingBox;
        private bool InSupply;
        /// <summary>
        /// Procent powietrza użytego ponownie
        /// </summary>

        private double _MixingPercent;

        public double MixingPercent
        {
            get { return _MixingPercent; }
            set
            {
                if(_MixingPercent != value)
                {
                    _MixingPercent = value;
                    CoupledMixingBox.MixingPercent = value;
                    OnPropertyChanged("MixingPercent");
                }
            }
        }

        public HVACMixingBox(bool inSupply)
        {
            IsGenerativeFlow = false;
            Name = "Komora mieszania";
            IsMovable = false;
            IsMutable = true;
            InSupply = inSupply;
            ImageSource = @"images\mixingbox.png";
            SetPlotDataNames();
            SetInitialValuesParameters();
            
            InitializeParametersList();
        }

        public override bool IsPresent
        {
            get => base.IsPresent;
            set
            {
                if (value != IsPresent)
                {
                    _IsPresent = value;
                    OnPropertyChanged("IsPresent");
                    if (CoupledMixingBox != null) 
                    {
                        CoupledMixingBox.IsPresent = value;
                    }
                }
            }
        }

        public List<BindableAnalogInputPort> BindedInputs { get; set; }

        public void CoupleMixingBox(HVACMixingBox mixingBox)
        {
            CoupledMixingBox = mixingBox;
        }

        public override Air CalculateOutputAirParameters(Air inputAir, ref double airFlow, ref double massFlow)
        {
            if(InSupply)
            {
                double specHum = (inputAir.SpecificHumidity * (100 - MixingPercent) +
                    CoupledMixingBox.OutputAir.SpecificHumidity * MixingPercent) / 100; //średnia ważona
                double temp = (inputAir.Temperature * (100 - MixingPercent) +
                    CoupledMixingBox.OutputAir.Temperature * MixingPercent) / 100; //średnia ważona
                OutputAir = new Air(temp, specHum, EAirHum.specific);
                AddDataPointFromAir(OutputAir, EDataType.temperature);
                AddDataPointFromAir(OutputAir, EDataType.humidity);

                airFlow *=  100 / (100 - MixingPercent);
                massFlow *= 100 / (100 - MixingPercent);

                return OutputAir;
            }
            else
            {
                airFlow *= (100 - MixingPercent) / 100;
                massFlow *= (100 - MixingPercent) / 100;
                return base.CalculateOutputAirParameters(inputAir, ref airFlow, ref massFlow);
            }
        }

        public void SetParameter(int parameter, EAnalogInput analogInput)
        {
            var bindedParameter = BindedInputs.FirstOrDefault(item => item.AnalogInput == analogInput);
            if (bindedParameter == null)
            {
                OnSimulationErrorOccured(string.Format("Próba przypisania nieprawidłowego parametru jako wysterowanie komory mieszania: {0}", analogInput.ToString()));
                return;
            }
            if (!bindedParameter.ValidateValue(parameter))
            {
                OnSimulationErrorOccured(string.Format("Niewłaściwa wartość parametru: {0}", parameter));
            }
            MixingPercent = bindedParameter.ConvertToParameterRange(parameter);
        }

        public void InitializeParametersList()
        {
            BindedInputs = new List<BindableAnalogInputPort>
            {
                new BindableAnalogInputPort(0, 100, true, EAnalogInput.mixingBox)
            };
        }

        public List<EAnalogInput> GetListOfParams(bool onlyVisible)
        {
            if (onlyVisible) return BindedInputs.Where(item => item.Visibility == true).Select(item => item.AnalogInput).ToList();
            return BindedInputs.Select(item => item.AnalogInput).ToList();
        }

        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();

            MixingPercent = 0;
        }
    }
}
