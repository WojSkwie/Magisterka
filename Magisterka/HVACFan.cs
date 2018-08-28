﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HVACSimulator
{
    public sealed class HVACFan : HVACObject, IDynamicObject, IBindableAnalogInput, IBindableDigitalInput
    {
        public HVACFan(bool inverted) : base()
        {
            IsGenerativeFlow = true;
            Name = "Wentylator";
            IsMovable = true;
            IsMutable = false;

            HasSingleTimeConstant = true;
            if(inverted)
            {
                ImageSource = @"images\fan2.png";
            }
            else
            {
                ImageSource = @"images\fan1.png";
            }
            SetPlotDataNames();
            InitializeParametersList();
            SetInitialValuesParameters();
        }

        private double _SetSpeedPercent;

        public double SetSpeedPercent
        {
            get { return _SetSpeedPercent; }
            set
            {
                _SetSpeedPercent = value;
                OnPropertyChanged("SetSpeedPercent");
            }
        }
        
        public double ActualSpeedPercent { get; set; }
        public List<BindableAnalogInputPort> BindedInputs { get; set; }
        private bool ActivateFan;
        List<EDigitalInput> IBindableDigitalInput.ParamsList { get; set; } = new List<EDigitalInput>
        {
            EDigitalInput.fanStart
        };

        public List<EAnalogInput> GetListOfParams()
        {
            return BindedInputs.Select(item => item.AnalogInput).ToList();
        }

        public void InitializeParametersList()
        {
            BindedInputs = new List<BindableAnalogInputPort>
            {
                new BindableAnalogInputPort(0.01, 100, EAnalogInput.fanSpeed)
            };

        }

        public void SetParameter(int parameter, EAnalogInput analogInput)
        {
            var bindedParameter = BindedInputs.FirstOrDefault(item => item.AnalogInput == analogInput);
            if(bindedParameter == null)
            {
                OnSimulationErrorOccured(string.Format("Próba przypisania nieprawidłowego parametru jako wysterowanie wentylatora: {0}", analogInput.ToString()));
                return;
            }
            if (!bindedParameter.ValidateValue(parameter))
            {
                OnSimulationErrorOccured(string.Format("Niewłaściwa wartość parametru: {0}", parameter));
            }
            SetSpeedPercent = bindedParameter.ConvertToParameterRange(parameter);
            
        }

        void IBindableDigitalInput.SetDigitalParameter(bool state, EDigitalInput digitalInput)
        {
            switch (digitalInput)
            {
                case EDigitalInput.fanStart:
                    ActivateFan = state;
                    break;
                default:
                    OnSimulationErrorOccured(string.Format("Próba ustawienia stanu nieistniejącego parametru w wentylatorze: {0}", digitalInput));
                    break;
            }
        }

        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();

            ACoeff = -1;
            BCoeff = 1;
            CCoeff = 120;
            ActualSpeedPercent = 0.01;
            SetSpeedPercent = 0.01;
            TimeConstant = 5;
        }

        public double CalculateDerivative(EVariableName variableName, double variableToDerivate)
        {
            switch (variableName)
            {
                case EVariableName.fanSpeed:
                    return (SetSpeedPercent - variableToDerivate) / TimeConstant;
                default:
                    OnSimulationErrorOccured(string.Format("Próba całkowania niewłaściwego obiektu w wentylatorze: {0}", variableName));
                    return 0;
            }
        }

        public void UpdateParams()
        {
            if (TimeConstant <= 0)
            {
                OnSimulationErrorOccured("Nieprawidłowa stała czasowa");
                return;
            }

            double startDerivative = CalculateDerivative(EVariableName.fanSpeed, ActualSpeedPercent);
            double midValue = ActualSpeedPercent + (startDerivative * Constants.step / 2.0);
            double midDerivative = CalculateDerivative(EVariableName.fanSpeed, midValue);
            ActualSpeedPercent += midDerivative * Constants.step;

        }
    }
}
