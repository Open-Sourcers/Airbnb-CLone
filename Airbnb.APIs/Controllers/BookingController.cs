﻿using System.Net;
using System.Security.Claims;
using Airbnb.Domain;
using Airbnb.Domain.DataTransferObjects;
using Airbnb.Domain.Entities;
using Airbnb.Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.APIs.Controllers
{
    public class BookingController : APIBaseController
    {
        private readonly IBookService _bookService;
        private readonly IValidator<BookingToCreateDTO> _bookingToCreateValidator;

        public BookingController(IBookService bookService, IValidator<BookingToCreateDTO> bookingToCreateValidator)
        {
            _bookService = bookService;
            _bookingToCreateValidator = bookingToCreateValidator;
        }
        //[Authorize]
        [HttpPost("CreateBooking")]
        public async Task<ActionResult<Responses>> CreateBooking([FromBody] BookingToCreateDTO bookDTO)
        {
            var validate = await _bookingToCreateValidator.ValidateAsync(bookDTO);
            if (!validate.IsValid) return await Responses.FailurResponse(validate.Errors, HttpStatusCode.BadRequest);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email is null)
            {
                return await Responses.FailurResponse("Owner is not found try again");
            }

            return Ok(await _bookService.CreateBookingByPropertyId(email, bookDTO));
        }

        [HttpDelete("CancelBooking")]
        public async Task<ActionResult<Responses>> CancelBooking([FromQuery] int bookingId)
        {
            return Ok(await _bookService.DeleteBookingById(bookingId));
        }


        [HttpGet("GetBooking")]
        public async Task<ActionResult<Responses>> GetBookingById([FromQuery] int bookingId)
        {
            return Ok(await _bookService.GetBookingById(bookingId));
        }


        [HttpGet("GetBookingsByUser")]
        public async Task<ActionResult<Responses>> GetBookingsByUserId([FromQuery] string userId)
        {
            return Ok(await _bookService.GetBookingsByUserId(userId));
        }

        [HttpPut("UpdateBookingById")]
        public async Task<ActionResult<Responses>> UpdateBookingByPropertyId(int bookingId, BookingToUpdateDTO bookDto)
        {
            return Ok(await _bookService.UpdateBookingByPropertyId(bookingId, bookDto));
        }
    }
}
