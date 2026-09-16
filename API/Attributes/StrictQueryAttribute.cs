using Microsoft.AspNetCore.Mvc.ActionConstraints;

public class StrictQueryAttribute : Attribute, IActionConstraint
{
    // Order 0 means it runs during the initial route matching
    public int Order => 0;

    public bool Accept(ActionConstraintContext context)
    {
        var query = context.RouteContext.HttpContext.Request.Query;
        if (query.Count == 0) return true;

        var actionParams = context.CurrentCandidate.Action.Parameters;

        foreach (var key in query.Keys)
        {
            bool found = false;
            // Use a simple for-loop to avoid Iterator allocations
            for (int i = 0; i < actionParams.Count; i++)
            {
                if (string.Equals(actionParams[i].Name, key, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }
        return true;
    }
}