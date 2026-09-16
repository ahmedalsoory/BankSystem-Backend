

using API.Middleware;

namespace API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseHttpsRedirection();

            // 🚀 CRITICAL: Static files should be served early in the pipeline
            // before routing and custom auth/protocol checks intercept image requests.
            app.UseStaticFiles();

            app.UseCors();
            app.UseMiddleware<ProtocolCheckMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.Use(async (context, next) =>
            {
                try
                {
                    Console.WriteLine($"Request Path: {context.Request.Path}");
                    await next();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"!!! PIPELINE ERROR: {ex.Message}");
                    Console.WriteLine($"!!! STACK TRACE: {ex.StackTrace}");
                    throw;
                }
            });

            app.UseRouting();

            // app.UseAuthentication(); 
            // app.UseAuthorization(); 

            return app;
        }
    }
}
