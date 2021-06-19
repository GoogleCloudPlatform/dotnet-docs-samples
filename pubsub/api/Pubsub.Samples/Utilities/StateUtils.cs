namespace AvroUtilities
{
    public class StateUtils
    {
        public static string StateToJsonString(AvroUtilities.State state)
        {
            return $"{{\"name\": \"{state.name}\", \"post_abbr\": \"{state.post_abbr}\"}}".ToString();
        }
    }
}
