using InterviewPrep.API.DTOs;
using InterviewPrep.API.Helpers;
using InterviewPrep.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPrep.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;
        private readonly JwtHelper _jwtHelper;

        public NotesController(INoteService noteService, JwtHelper jwtHelper)
        {
            _noteService = noteService;
            _jwtHelper = jwtHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotes()
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            var notes = await _noteService.GetAllAsync(userId);
            return Ok(notes);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetNote(Guid id)
        {
            try
            {
                var userId = _jwtHelper.GetUserIdFromToken(User);
                var note = await _noteService.GetByIdAsync(id, userId);
                return Ok(note);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Note not found." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] CreateNoteDto dto)
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            var note = await _noteService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetNote), new { id = note.NoteId }, note);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateNote(Guid id, [FromBody] UpdateNoteDto dto)
        {
            try
            {
                var userId = _jwtHelper.GetUserIdFromToken(User);
                var note = await _noteService.UpdateAsync(id, userId, dto);
                return Ok(note);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Note not found." });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            try
            {
                var userId = _jwtHelper.GetUserIdFromToken(User);
                await _noteService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Note not found." });
            }
        }
    }
}
