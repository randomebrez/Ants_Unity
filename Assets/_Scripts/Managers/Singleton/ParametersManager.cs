using Assets._Scripts.Utilities;
using Assets.Dtos;
using mew;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Singleton whose role is to communicate with the static class GlobalParameters.
// It should remain the only one able to set simulation parameters.
// Therefore it should have the connection to the database I guess
internal  class ParametersManager : BaseManager<ParametersManager>
{
    private List<SimulationParameterTypeEnum> _simulationParameters = new List<SimulationParameterTypeEnum>
    {
        SimulationParameterTypeEnum.MaxPopulation,
        SimulationParameterTypeEnum.LifeTime,
        SimulationParameterTypeEnum.SelectPercent,
        SimulationParameterTypeEnum.ChildMean,
        //SimulationParameterTypeEnum.StorageFrequency
    };

    private List<NeuralNetworkParameterTypeEnum> _neuralNetworkParameters = new List<NeuralNetworkParameterTypeEnum>
    {
        //NeuralNetworkParameterTypeEnum.NeutralLayers,
        //NeuralNetworkParameterTypeEnum.GeneWeightBitNumber,
        //NeuralNetworkParameterTypeEnum.GeneNumber,
        NeuralNetworkParameterTypeEnum.SelectedBrainGraph
    };

    private List<AntCaracteristicsParameterTypeEnum> _antCaracteristics = new List<AntCaracteristicsParameterTypeEnum>
    {
        AntCaracteristicsParameterTypeEnum.VisionRadius,
        AntCaracteristicsParameterTypeEnum.VisionAngle,
        AntCaracteristicsParameterTypeEnum.PheroDurationW,
        AntCaracteristicsParameterTypeEnum.PheroDurationC
    };


    // Simulation parameters

    public Dictionary<SimulationParameterTypeEnum, (string value, SimulationParameterConstraint valueConstraint)> SimulationParametersGet()
    {
        var result = new Dictionary<SimulationParameterTypeEnum, (string value, SimulationParameterConstraint valueConstraint)>();

        for(int i = 0; i < _simulationParameters.Count; i++)
            result.Add(_simulationParameters[i], (SimulationParameterValueGet(_simulationParameters[i]), SimulationParameterConstraintGet(_simulationParameters[i])));

        return result;
    }

    public void SimulationParametersSave(Dictionary<SimulationParameterTypeEnum, string> newValues)
    {
        foreach (var pair in newValues)
            SimulationParameterValueSet(pair.Key, pair.Value);
    }

    private string SimulationParameterValueGet(SimulationParameterTypeEnum type)
    {
        switch(type)
        {
            case SimulationParameterTypeEnum.MaxPopulation:
                return GlobalParameters.ColonyMaxPopulation.ToString();                
            case SimulationParameterTypeEnum.LifeTime:
                return GlobalParameters.LifeTimeFrame.ToString();
            case SimulationParameterTypeEnum.SelectPercent:
                return GlobalParameters.ReproductionCaracteristics.PercentToSelect.ToString();
            case SimulationParameterTypeEnum.ChildMean:
                return GlobalParameters.ReproductionCaracteristics.MeanChildNumberByUnit.ToString();
            //case SimulationParameterTypeEnum.StorageFrequency:
            //    return GlobalParameters.StoreFrequency.ToString();
            default:
                return string.Empty;
        }
    }

    private SimulationParameterConstraint SimulationParameterConstraintGet(SimulationParameterTypeEnum type)
    {
        var constraint = new SimulationParameterConstraint();
        switch (type)
        {
            case SimulationParameterTypeEnum.MaxPopulation:
                constraint.MinValue = 5;
                constraint.MaxValue = 1000;
                break;
            case SimulationParameterTypeEnum.LifeTime:
                constraint.MinValue = 1;
                constraint.MaxValue = 10000;
                break;
            case SimulationParameterTypeEnum.SelectPercent:
                constraint.MinValue = 1;
                constraint.MaxValue = 100;
                break;
            case SimulationParameterTypeEnum.ChildMean:
                constraint.MinValue = 1;
                constraint.MaxValue = 10;
                break;
            //case SimulationParameterTypeEnum.StorageFrequency:
            //    constraint.MinValue = 1;
            //    constraint.MaxValue = 1000;
            //    break;
        }

        return constraint;
    }

    private void SimulationParameterValueSet(SimulationParameterTypeEnum type, string value)
    {
        switch (type)
        {
            case SimulationParameterTypeEnum.MaxPopulation:
                GlobalParameters.ColonyMaxPopulation = int.Parse(value);
                break;
            case SimulationParameterTypeEnum.LifeTime:
                GlobalParameters.LifeTimeFrame = int.Parse(value);
                break;
            case SimulationParameterTypeEnum.SelectPercent:
                GlobalParameters.ReproductionCaracteristics.PercentToSelect = int.Parse(value);
                break;
            case SimulationParameterTypeEnum.ChildMean:
                GlobalParameters.ReproductionCaracteristics.MeanChildNumberByUnit = int.Parse(value);
                break;
            //case SimulationParameterTypeEnum.StorageFrequency:
            //    GlobalParameters.StoreFrequency = int.Parse(value);
            //    break;
        }
    }



    // NeuralNetwork parameters

    public Dictionary<NeuralNetworkParameterTypeEnum, (string value, SimulationParameterConstraint valueConstraint)> NeuralNetworkParametersGet()
    {
        var result = new Dictionary<NeuralNetworkParameterTypeEnum, (string value, SimulationParameterConstraint valueConstraint)>();

        for (int i = 0; i < _neuralNetworkParameters.Count; i++)
            result.Add(_neuralNetworkParameters[i], (NeuralNetworkParameterValueGet(_neuralNetworkParameters[i]), NeuralNetworkParameterConstraintGet(_neuralNetworkParameters[i])));

        return result;
    }

    public void NeuralNetworkParametersSave(Dictionary<NeuralNetworkParameterTypeEnum, string> newValues)
    {
        foreach (var pair in newValues)
            NeuralNetworkParameterValueSet(pair.Key, pair.Value);
    }

    private string NeuralNetworkParameterValueGet(NeuralNetworkParameterTypeEnum type)
    {
        //return string.Empty;
        switch (type)
        {
            //    case NeuralNetworkParameterTypeEnum.NeutralLayers:
            //        var result = new StringBuilder();
            //        for (int i = 0; i < GlobalParameters.BrainCaracteristics.NeutralNumbers.Count; i ++)
            //        {
            //            if (i == GlobalParameters.NetworkCaracteristics.NeutralNumbers.Count - 1)
            //                result.Append($"{GlobalParameters.NetworkCaracteristics.NeutralNumbers[i]}");
            //            else
            //                result.Append($"{GlobalParameters.NetworkCaracteristics.NeutralNumbers[i]};");
            //        }
            //        return result.ToString();
            //    case NeuralNetworkParameterTypeEnum.GeneWeightBitNumber:
            //        return GlobalParameters.NetworkCaracteristics.WeighBytesNumber.ToString();
            //    case NeuralNetworkParameterTypeEnum.GeneNumber:
            //        return GlobalParameters.NetworkCaracteristics.GeneNumber.ToString();
            case NeuralNetworkParameterTypeEnum.SelectedBrainGraph:
                return GlobalParameters.SelectedBrainGraph;
            default:
                return string.Empty;
        }
    }

    private SimulationParameterConstraint NeuralNetworkParameterConstraintGet(NeuralNetworkParameterTypeEnum type)
    {
        var constraint = new SimulationParameterConstraint();
        switch (type)
        {
            //case NeuralNetworkParameterTypeEnum.NeutralLayers:
            //    constraint.RegExp = "^[0-9]+(;[0-9]+)*$";
            //    break;
            //case NeuralNetworkParameterTypeEnum.GeneWeightBitNumber:
            //    constraint.MinValue = 1;
            //    constraint.MaxValue = 10;
            //    break;
            //case NeuralNetworkParameterTypeEnum.GeneNumber:
            //    constraint.MinValue = 1;
            //    constraint.MaxValue = 1000;
            //    break;
            case NeuralNetworkParameterTypeEnum.SelectedBrainGraph:
                constraint.PossibleValues = new HashSet<string> { "Splitted", "BigBrain"};
                break;
        }

        return constraint;
    }

    private void NeuralNetworkParameterValueSet(NeuralNetworkParameterTypeEnum type, string value)
    {
        switch (type)
        {
            //    case NeuralNetworkParameterTypeEnum.NeutralLayers:
            //        var result = new List<int>();
            //        var values = value.Split(';');
            //        for (int i = 0; i < values.Where(t => string.IsNullOrEmpty(t) == false).Count(); i++)
            //            result.Add(int.Parse(values[i]));
            //        GlobalParameters.NetworkCaracteristics.NeutralNumbers = result;
            //        break;
            //    case NeuralNetworkParameterTypeEnum.GeneWeightBitNumber:
            //        GlobalParameters.NetworkCaracteristics.WeighBytesNumber = int.Parse(value);
            //        break;
            //    case NeuralNetworkParameterTypeEnum.GeneNumber:
            //        GlobalParameters.NetworkCaracteristics.GeneNumber = int.Parse(value);
            //        break;
            case NeuralNetworkParameterTypeEnum.SelectedBrainGraph:
                GlobalParameters.SelectedBrainGraph = value;
                break;
        }
    }



    // AntCaracteristics parameters

    public Dictionary<AntCaracteristicsParameterTypeEnum, (string value, SimulationParameterConstraint valueConstraint)> AntCaracteristicsParametersGet()
    {
        var result = new Dictionary<AntCaracteristicsParameterTypeEnum, (string value, SimulationParameterConstraint valueConstraint)>();

        for (int i = 0; i < _antCaracteristics.Count; i++)
            result.Add(_antCaracteristics[i], (NeuralNetworkParameterValueGet(_antCaracteristics[i]), AntCaracteristicsParameterConstraintGet(_antCaracteristics[i])));

        return result;
    }

    public void AntCaracteristicsParametersSave(Dictionary<AntCaracteristicsParameterTypeEnum, string> newValues)
    {
        foreach (var pair in newValues)
            AntCaracteristicsParameterValueSet(pair.Key, pair.Value);
    }

    private string NeuralNetworkParameterValueGet(AntCaracteristicsParameterTypeEnum type)
    {
        switch (type)
        {
            case AntCaracteristicsParameterTypeEnum.VisionRadius:
                return GlobalParameters.VisionRange.ToString();
            case AntCaracteristicsParameterTypeEnum.VisionAngle:
                return GlobalParameters.VisionAngle.ToString();
            case AntCaracteristicsParameterTypeEnum.PheroDurationW:
                return ResourceSystem.Instance.PheromoneOfTypeGet(ScriptablePheromoneBase.PheromoneTypeEnum.Wander).BaseCaracteristics.Duration.ToString();
            case AntCaracteristicsParameterTypeEnum.PheroDurationC:
                return ResourceSystem.Instance.PheromoneOfTypeGet(ScriptablePheromoneBase.PheromoneTypeEnum.CarryFood).BaseCaracteristics.Duration.ToString();
            default:
                return string.Empty;
        }
    }

    private SimulationParameterConstraint AntCaracteristicsParameterConstraintGet(AntCaracteristicsParameterTypeEnum type)
    {
        var constraint = new SimulationParameterConstraint();
        switch (type)
        {
            case AntCaracteristicsParameterTypeEnum.VisionRadius:
                constraint.MinValue = 1;
                constraint.MaxValue = 10;
                break;
            case AntCaracteristicsParameterTypeEnum.VisionAngle:
                constraint.MinValue = 0;
                constraint.MaxValue = 360;
                break;
            case AntCaracteristicsParameterTypeEnum.PheroDurationW:
                constraint.MinValue = 1;
                constraint.MaxValue = 1000;
                break;
            case AntCaracteristicsParameterTypeEnum.PheroDurationC:
                constraint.MinValue = 1;
                constraint.MaxValue = 1000;
                break;
        }

        return constraint;
    }

    private void AntCaracteristicsParameterValueSet(AntCaracteristicsParameterTypeEnum type, string value)
    {
        switch (type)
        {
            case AntCaracteristicsParameterTypeEnum.VisionRadius:
                ResourceSystem.Instance.ModifyVisionRadius(ScriptableAntBase.AntTypeEnum.Worker, int.Parse(value));
                GlobalParameters.VisionRange = int.Parse(value);
                break;
            case AntCaracteristicsParameterTypeEnum.VisionAngle:
                ResourceSystem.Instance.ModifyVisionAngle(ScriptableAntBase.AntTypeEnum.Worker, int.Parse(value));
                GlobalParameters.VisionAngle = int.Parse(value);
                break;
            case AntCaracteristicsParameterTypeEnum.PheroDurationW:
                ResourceSystem.Instance.ModifyPheromoneDuration(ScriptablePheromoneBase.PheromoneTypeEnum.Wander, int.Parse(value));
                break;
            case AntCaracteristicsParameterTypeEnum.PheroDurationC:
                ResourceSystem.Instance.ModifyPheromoneDuration(ScriptablePheromoneBase.PheromoneTypeEnum.CarryFood, int.Parse(value));
                break;
        }
    }
}
