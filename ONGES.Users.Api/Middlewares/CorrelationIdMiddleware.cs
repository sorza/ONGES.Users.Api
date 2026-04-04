namespace ONGES.Users.Api.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {            
            if (!context.Request.Headers.TryGetValue("X-Correlation-Id", out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers.Append("X-Correlation-Id", correlationId);
            }
            
            context.Response.Headers.Append("X-Correlation-Id", correlationId);

            context.Items["CorrelationId"] = correlationId;

            await _next(context);
        }
    }
}
