namespace FSH.BlazorWebAssembly.Shared.Wrapper
{
    public interface IResult
    {
        List<string> Messages { get; set; }

        bool Succeeded { get; set; }

        public string Exception { get; set; }
    }

    public interface IResult<out T> : IResult
    {
        T Data { get; }
    }
}