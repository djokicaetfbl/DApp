﻿using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public MessagesController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDTO createMessageDTO)
        {
            var username = User.GetUsername();

            if (username == createMessageDTO.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var recipient = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message // vjerovatno bi trebalo kasnije neki manager napraviti pa u njemu pripremati modelske objekte
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };

            _uow.MessageRepository.AddMessage(message);

            if (await _uow.Complete())
                return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            // moramo obrisati sa obe strane (oba korisnika) poruku

            var username = User.GetUsername();

            var message = await _uow.MessageRepository.GetMessage(id);

            if (message.SenderUsername != username && message.RecipientUsername != username)
                return Unauthorized();

            if (message.SenderUsername == username)
                message.SenderDeleted = true;
            if(message.RecipientUsername == username)
                message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
                _uow.MessageRepository.DeleteMessage(message);

            if (await _uow.Complete())
                return Ok();

            return BadRequest("Problem deleting the message");

        }
    }
}
