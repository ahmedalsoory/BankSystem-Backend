using API.Filter;
using Microsoft.AspNetCore.Mvc;

namespace API.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ImageCommitAttribute : TypeFilterAttribute
    {
        public ImageCommitAttribute() : base(typeof(ImageCommitActionFilter))
        {
            // Set order to 4 so it executes inside GlobalConnectionFilter (Order 3)
            Order = 4;
        }
    }
}
