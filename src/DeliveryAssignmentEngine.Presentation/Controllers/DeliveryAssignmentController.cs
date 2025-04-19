using Microsoft.AspNetCore.Mvc;

namespace DeliveryAssignmentEngine.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveryAssignmentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly DeliveryAssignmentApplicationService _applicationService;

    public DeliveryAssignmentController(
        IMediator mediator,
        DeliveryAssignmentApplicationService applicationService)
    {
        _mediator = mediator;
        _applicationService = applicationService;
    }

    [HttpPost("assign")]
    public async Task<ActionResult<AssignmentResultDto>> AssignDeliveryToAgent([FromBody] AssignDeliveryAgentCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccessful)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("suitable-agents/{deliveryId}")]
    public async Task<ActionResult<IEnumerable<Guid>>> GetSuitableAgents(Guid deliveryId)
    {
        var query = new FindSuitableAgentsQuery { DeliveryId = deliveryId };
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPost("assign-best/{deliveryId}")]
    public async Task<ActionResult<DeliveryAssignmentDto>> AssignBestAgent(Guid deliveryId)
    {
        try
        {
            var result = await _applicationService.AssignBestAgentToDeliveryAsync(deliveryId);

            if (!result.IsSuccessful)
                return BadRequest(result);

            return Ok(result);
        }
        catch (DeliveryNotFoundException)
        {
            return NotFound(new { Message = $"Delivery with ID {deliveryId} not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while processing your request", Error = ex.Message });
        }
    }
}