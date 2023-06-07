namespace Assets.Dtos
{
    public class InputTypeBrainOutput : InputTypeBase
    {
        public override InputTypeEnum InputType => InputTypeEnum.BrainOutput;

        public string BrainName { get; set; }
    }
}
