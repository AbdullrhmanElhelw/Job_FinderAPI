﻿using Application.Abstractions;
using Application.DapperQueries.JobQueries;
using Application.Interfaces.UnitOfWork;
using Domain.Models;
using Domain.Shared;

namespace Application.Features.Commands.JobCommands.PartialUpdate;

public sealed class JopPartialUpdateCommandHandler(IUnitOfWork unitOfWork, IJobQuery jobQuery)
    : ICommandHandler<JopPartialUpdateCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJobQuery _jobQuery = jobQuery;

    public async Task<Result> Handle(JopPartialUpdateCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobQuery.GetToUpdate(request.Id);

        if (job is null)
            return Result.Fail("Job not found");

        var jobToUpdate =
            new Job
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                EmployerId = job.EmployerId
            };

        request.JobPD.ApplyTo(jobToUpdate);

        _unitOfWork.JobRepository.UpdateAsync(jobToUpdate);

        if (await _unitOfWork.CommitAsync() == 0)
            return Result.Fail("Failed to update job");

        return Result.Ok();
    }
}