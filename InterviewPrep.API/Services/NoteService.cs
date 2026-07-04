using InterviewPrep.API.Data;
using InterviewPrep.API.DTOs;
using InterviewPrep.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewPrep.API.Services
{
    public interface INoteService
    {
        Task<List<NoteResponseDto>> GetAllAsync(Guid userId);
        Task<NoteResponseDto> GetByIdAsync(Guid noteId, Guid userId);
        Task<NoteResponseDto> CreateAsync(Guid userId, CreateNoteDto dto);
        Task<NoteResponseDto> UpdateAsync(Guid noteId, Guid userId, UpdateNoteDto dto);
        Task DeleteAsync(Guid noteId, Guid userId);
    }

    public class NoteService : INoteService
    {
        private readonly AppDbContext _context;

        public NoteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<NoteResponseDto>> GetAllAsync(Guid userId)
        {
            return await _context.Notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => MapToDto(n))
                .ToListAsync();
        }

        public async Task<NoteResponseDto> GetByIdAsync(Guid noteId, Guid userId)
        {
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);

            if (note == null)
                throw new KeyNotFoundException("Note not found.");

            return MapToDto(note);
        }

        public async Task<NoteResponseDto> CreateAsync(Guid userId, CreateNoteDto dto)
        {
            var note = new Note
            {
                UserId = userId,
                Title = dto.Title.Trim(),
                Content = dto.Content.Trim(),
                Topic = dto.Topic.Trim()
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return MapToDto(note);
        }

        public async Task<NoteResponseDto> UpdateAsync(Guid noteId, Guid userId, UpdateNoteDto dto)
        {
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);

            if (note == null)
                throw new KeyNotFoundException("Note not found.");

            if (dto.Title != null) note.Title = dto.Title.Trim();
            if (dto.Content != null) note.Content = dto.Content.Trim();
            if (dto.Topic != null) note.Topic = dto.Topic.Trim();

            note.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(note);
        }

        public async Task DeleteAsync(Guid noteId, Guid userId)
        {
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);

            if (note == null)
                throw new KeyNotFoundException("Note not found.");

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }

        private static NoteResponseDto MapToDto(Note n) => new()
        {
            NoteId = n.NoteId,
            Title = n.Title,
            Content = n.Content,
            Topic = n.Topic,
            CreatedDate = n.CreatedDate,
            UpdatedDate = n.UpdatedDate
        };
    }
}
