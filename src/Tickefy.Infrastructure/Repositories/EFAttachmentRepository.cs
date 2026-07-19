using Microsoft.EntityFrameworkCore;
using Tickefy.Domain.Attachments;
using Tickefy.Domain.Primitives;
using Tickefy.Infrastructure.Database;

namespace Tickefy.Infrastructure.Repositories;

public class EFAttachmentRepository : IAttachmentRepository
{
    private readonly AppDbContext _context;

    public EFAttachmentRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Attachment attachment)
    {
        await _context.Attachments.AddAsync(attachment);
    }

    public Task<Attachment?> GetByIdAsync(AttachmentId id)
    {
        return _context.Attachments.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task DeleteAsync(AttachmentId id)
    {
        var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == id);
        if (attachment is null)
        {
            return;
        }

        _context.Attachments.Remove(attachment);
    }

    public void Update(Attachment attachment)
    {
        _context.Attachments.Update(attachment);
    }
}
