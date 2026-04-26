using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;

namespace TaskApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;

    public TasksController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<TaskItem>>> GetTasks()
    {
        var tasks = await _db.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskItem>> UpdateTask(int id, UpdateTaskDto dto)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Priority = dto.Priority;
        task.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(task);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<TaskItem>> PatchTask(int id, PatchTaskDto dto)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        if (dto.Title != null) task.Title = dto.Title;
        if (dto.Description != null) task.Description = dto.Description;
        if (dto.Priority != null) task.Priority = dto.Priority;
        if (dto.Status != null) task.Status = dto.Status;
        task.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
