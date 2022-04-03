using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using server.Models;

namespace server.Hubs
{
    public interface IPostHub
    {
        Task PostAdded(Post model);
        Task PostScoreChange(Guid postId, int score);
        Task PostCountChange(Guid postId, int commentCount);
        Task CommentAdded(Comment comment);
    }

    [Authorize]
    public class PostHub: Hub<IPostHub>
    {
        public async Task JoinPostGroup(Guid postId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, postId.ToString());
        }
        public async Task LeavePostGroup(Guid postId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, postId.ToString());
        }
    }
}