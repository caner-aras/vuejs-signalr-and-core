using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using server.Hubs;
using server.Models;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IHubContext<PostHub, IPostHub> hubContext;
        private static ConcurrentBag<Post> post = new ConcurrentBag<Post> {
          new Post {
                Id = Guid.NewGuid(),
                CreatedBy = "caneraras@yahoo.com",
                Title = "Welcome to basic blog",
                Body = "Im Caner ARAS! Welcome to my basic blog.\n" +
                       " - [LinkedIn Profile](https://www.linkedin.com/in/caner-aras)\n" ,
                Comments = new List<Comment>{ new Comment { Body = "this is sample comment", CreatedBy ="caneraras@yahoo.com" }}
            },
        };

        public PostController(IHubContext<PostHub, IPostHub> postHub)
        {
            this.hubContext = postHub;
        }

        [HttpGet()]
        public IEnumerable GetPost()
        {
            return post.Where(t => !t.Deleted).Select(q => new
            {
                q.Id,
                q.CreatedBy,
                q.Title,
                q.Body,
                q.Score,
                AnswerCount = q.Comments.Count
            });
        }

        [HttpGet("{id}")]
        public ActionResult GetPost(Guid id)
        {
            var model = post.SingleOrDefault(t => t.Id == id && !t.Deleted);
            if (model == null) return NotFound();

            return new JsonResult(model);
        }

        [HttpPost()]
        [Authorize]
        public async Task<Post> AddPost([FromBody]Post model)
        {
            model.Id = Guid.NewGuid();
            model.CreatedBy = this.User.Identity.Name;
            model.Deleted = false;
            model.Comments = new List<Comment>();
            post.Add(model);
            await this.hubContext.Clients.All.PostAdded(model);
            return model;
        }

        [HttpPost("{id}/comments")]
        [Authorize]
        public async Task<ActionResult> AddAnswerAsync(Guid id, [FromBody]Comment comment)
        {
            var blogData = post.SingleOrDefault(t => t.Id == id && !t.Deleted);
            if (blogData == null) return NotFound();

            comment.Id = Guid.NewGuid();
            comment.PostId = id;
            comment.CreatedBy = this.User.Identity.Name;
            comment.Deleted = false;
            blogData.Comments.Add(comment);

            // Notify connected to the group for this answer
            await this.hubContext.Clients.Group(id.ToString()).CommentAdded(comment);
            // Notify every client
            await this.hubContext.Clients.All.PostCountChange(blogData.Id, blogData.Comments.Count);

            return new JsonResult(comment);
        }

        [HttpPatch("{id}/upvote")]
        [Authorize]
        public async Task<ActionResult> UpvotePostAsync(Guid id)
        {
            var obj = post.SingleOrDefault(t => t.Id == id && !t.Deleted);
            if (obj == null) return NotFound();

            obj.Score++;

            // Notify every client
            await this.hubContext.Clients.All.PostScoreChange(obj.Id, obj.Score);

            return new JsonResult(obj);
        }

        [HttpPatch("{id}/downvote")]
        [Authorize]
        public async Task<ActionResult> DownvotePostAsync(Guid id)
        {
            var model = post.SingleOrDefault(t => t.Id == id && !t.Deleted);
            if (model == null) return NotFound();

            model.Score--;

            // Notify every client
            await this.hubContext.Clients.All.PostScoreChange(model.Id, model.Score);

            return new JsonResult(model);
        }
    }
}
