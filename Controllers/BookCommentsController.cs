﻿using Bakalauras.Auth.Model;
using Bakalauras.Auth;
using Bakalauras.data.dtos;
using Bakalauras.data.repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Policy;
using Bakalauras.data.entities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Bakalauras.Controllers
{
    [ApiController]
    [Route("api/genres/{genreName}/books/{bookId}/comments")]
    public class BookCommentsController : ControllerBase
    {
        private readonly ICommentRepository _CommentRepository;
        private readonly UserManager<BookieUser> _UserManager;
        private readonly IAuthorizationService _AuthorizationService;
        private readonly IBookRepository _BookRepository;
        private const string _Type = "Book";
        public BookCommentsController(ICommentRepository repo, IAuthorizationService authService,
            UserManager<BookieUser> userManager, IBookRepository bookRepository)
        {
            _CommentRepository = repo;
            _AuthorizationService = authService;
            _UserManager = userManager;
            _BookRepository = bookRepository;
        }
        [HttpGet]
        public async Task<IEnumerable<CommentDto>> GetMany(int bookId)
        {
            var comments = await _CommentRepository.GetManyAsync(bookId,_Type);
            return comments.Select(x => new CommentDto(x.Id, x.EntityId, _Type, DateTime.Now, x.Content, x.UserId, x.Username));
        }

        [HttpGet]
        [Route("{commentId}")]
        public async Task<ActionResult<CommentDto>> Get(int bookId, int commentId)
        {
            var comment = await _CommentRepository.GetAsync(commentId, bookId,_Type);
            if (comment == null) return NotFound();
            return new CommentDto(comment.Id, comment.EntityId,_Type, DateTime.Now, comment.Content,
                comment.UserId, comment.Username);
        }

        [HttpPost]
        [Authorize(Roles = BookieRoles.BookieUser + "," + BookieRoles.Admin)]
        public async Task<ActionResult<CommentDto>> Create(CreateCommentDto createCommentDto, int bookId,string genreName)
        {
            var user = _UserManager.GetUserName(User);
            var comment = new Comment
            {
                EntityType=_Type,
                Content = createCommentDto.Content,
                Date = DateTime.Now,
                Username = user,
                UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            };

            var book = await _BookRepository.GetAsync(bookId, genreName);
            if (book == null) return NotFound();

            await _CommentRepository.CreateAsync(comment, bookId,_Type);

            //201
            return Created("201", new CommentDto(comment.Id, comment.EntityId,_Type, comment.Date, comment.Content,
                comment.UserId, comment.Username));
        }

        [HttpPut]
        [Route("{commentId}")]
        [Authorize(Roles = BookieRoles.BookieUser + "," + BookieRoles.Admin)]
        public async Task<ActionResult<CommentDto>> Update(int commentId, int bookId, UpdateCommentDto updateCommentDto)
        {
            var comment = await _CommentRepository.GetAsync(commentId, bookId,_Type);
            if (comment == null) return NotFound();
            var authRez = await _AuthorizationService.AuthorizeAsync(User, comment, PolicyNames.ResourceOwner);
            if (!authRez.Succeeded)
            {
                return Forbid();
            }
            comment.Content = updateCommentDto.Content;
            await _CommentRepository.UpdateAsync(comment);


            return Ok(new CommentDto(comment.Id, comment.EntityId,_Type, DateTime.Now, comment.Content, comment.UserId, comment.Username));
        }

        [HttpDelete]
        [Route("{commentId}")]
        public async Task<ActionResult> Remove(int commentId, int bookId)
        {
            var comment = await _CommentRepository.GetAsync(commentId, bookId, _Type);
            if (comment == null) return NotFound();
            await _CommentRepository.DeleteAsync(comment);

            //204
            return NoContent();
        }
    }
}