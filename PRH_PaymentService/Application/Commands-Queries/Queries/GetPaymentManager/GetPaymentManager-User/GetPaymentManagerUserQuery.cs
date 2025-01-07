using System;
using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPaymentManager.GetPaymentManager_User;

public record GetPaymentManagerUserQuery : IRequest<BaseResponse<IEnumerable<PaymentManagerUserDto>>>;