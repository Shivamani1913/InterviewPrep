namespace InterviewPrep.API.Middleware
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;

        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS, PATCH";
            context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization, Accept";

            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("OK");
                return;
            }

            await _next(context);
        }
    }
}
