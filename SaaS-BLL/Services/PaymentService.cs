using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SaaS_BLL.Common;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_Domain.Entities;
using SaaS_Domain.Enums;
using SaaS_Domain.Interfaces;

namespace SaaS_BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork uow, IMapper mapper)
    {
        this._uow = uow;
        this._mapper = mapper;
    }

    public async Task<ServiceResult<PaymentDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var payment = await this._uow.Payments.GetByIdAsync(id, ct);
        return payment is null ? ServiceResult<PaymentDto>.Fail("Payment not found.") : ServiceResult<PaymentDto>.Ok(this._mapper.Map<PaymentDto>(payment));
    }

    public async Task<ServiceResult<IEnumerable<PaymentDto>>> GetBySubscriptionIdAsync(int subscriptionId, CancellationToken ct = default)
    {
        var payments = await this._uow.Payments.Query()
            .Where(p => p.SubscriptionId == subscriptionId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(ct);

        return ServiceResult<IEnumerable<PaymentDto>>.Ok(this._mapper.Map<IEnumerable<PaymentDto>>(payments));
    }

    public async Task<ServiceResult<PaymentDto>> RecordPaymentAsync(RecordPaymentRequest request, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
        {
            return ServiceResult<PaymentDto>.Fail("Payment amount must be greater than zero.");
        }

        var subscriptionExists = await this._uow.Subscriptions.ExistsAsync(request.SubscriptionId, ct);
        if (!subscriptionExists)
        {
            return ServiceResult<PaymentDto>.Fail("Subscription not found.");
        }

        var payment = new Payment(
            id: 0,
            subscriptionId: request.SubscriptionId,
            amount: request.Amount,
            currency: request.Currency,
            paymentDate: DateTime.UtcNow,
            paymentMethod: request.PaymentMethod,
            status: PaymentStatus.Pending);

        await this._uow.Payments.AddAsync(payment, ct);
        await this._uow.SaveChangesAsync(ct);

        return ServiceResult<PaymentDto>.Ok(this._mapper.Map<PaymentDto>(payment));
    }

    public async Task<ServiceResult<PaymentDto>> UpdateStatusAsync(int id, PaymentStatus status, CancellationToken ct = default)
    {
        var payment = await this._uow.Payments.GetByIdAsync(id, ct);
        if (payment is null)
        {
            return ServiceResult<PaymentDto>.Fail("Payment not found.");
        }

        if (payment.Status == PaymentStatus.Refunded)
        {
            return ServiceResult<PaymentDto>.Fail("A refunded payment cannot be updated.");
        }

        payment.Status = status;
        this._uow.Payments.Update(payment);
        await this._uow.SaveChangesAsync(ct);

        return ServiceResult<PaymentDto>.Ok(this._mapper.Map<PaymentDto>(payment));
    }
}