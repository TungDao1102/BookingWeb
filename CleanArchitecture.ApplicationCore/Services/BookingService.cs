﻿using CleanArchitecture.ApplicationCore.Commons;
using CleanArchitecture.ApplicationCore.Entities;
using CleanArchitecture.ApplicationCore.Interfaces.Repositories;
using CleanArchitecture.ApplicationCore.Interfaces.Services;
using CleanArchitecture.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.ApplicationCore.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private ResponseDTO _response;
        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new ResponseDTO();
        }
        public async Task<ResponseDTO> CreateBooking(Booking booking)
        {
            try
            {
                _response.Result = await _unitOfWork.bookingRepo.AddAsync(booking);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDTO> GetAllBooking()
        {
            try
            {
                _response.Result = await _unitOfWork.bookingRepo.ListAsync();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDTO> GetBooking(int bookingId)
        {
            try
            {
                _response.Result = await _unitOfWork.bookingRepo.GetByIdAsync(bookingId);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDTO> UpdatePayment(int bookingId, string sessionId, string paymentIntentId)
        {
            try
            {
                Booking? booking = await _unitOfWork.bookingRepo.GetByIdAsync(bookingId);
                if(booking is not null)
                {
                    if (!string.IsNullOrEmpty(sessionId))
                    {
                        booking.StripeSessionId = sessionId;
                    }
                    if (!string.IsNullOrEmpty(paymentIntentId))
                    {
                        booking.StripePaymentIntentId = paymentIntentId;
                        booking.PaymentDate = DateTime.Now;
                        booking.IsPaymentSuccessful = true;
                    }
                    await _unitOfWork.bookingRepo.UpdateAsync(booking);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Not Found Booking!";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDTO> UpdateStatus(int bookingId, string status)
        {
            try
            {
                Booking? booking = await _unitOfWork.bookingRepo.GetByIdAsync(bookingId);
                if (booking is not null)
                {
                    booking.Status = status;
                    if (status == PaymentStatus.StatusCheckedIn)
                    {
                        booking.VillaNumber = 0;
                        booking.ActualCheckInDate = DateTime.Now;
                    }
                    if (status == PaymentStatus.StatusCompleted)
                    {
                        booking.ActualCheckOutDate = DateTime.Now;
                    }
                    await _unitOfWork.bookingRepo.UpdateAsync(booking);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
