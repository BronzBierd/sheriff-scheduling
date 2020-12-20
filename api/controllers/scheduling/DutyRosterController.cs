﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.scheduling;

namespace SS.Api.controllers.scheduling
{
    [Route("api/[controller]")]
    [ApiController]
    public class DutyRosterController : ControllerBase
    {
        public const string InvalidDutyErrorMessage = "Invalid Duty.";
        public const string CannotUpdateCrossLocationError = "Cannot update cross location.";
        private DutyRosterService DutyRosterService { get; }
        private SheriffDbContext Db { get; }
        public DutyRosterController(DutyRosterService dutyRosterService, SheriffDbContext db)
        {
            DutyRosterService = dutyRosterService;
            Db = db;
        }

        /// <summary>
        /// This is for the center of the DutyRoster screen. Specifically when assignments are created into Duties. 
        /// </summary>
        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.ViewDutyRoster)]
        public async Task<ActionResult<List<DutyDto>>> GetDuties(int locationId, DateTimeOffset start, DateTimeOffset end)
        {
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationId)) return Forbid();

            var duties = await DutyRosterService.GetDutiesForLocation(locationId, start, end);
            return Ok(duties.Adapt<List<DutyDto>>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignDuties)]
        public async Task<ActionResult<List<DutyDto>>> AddDuties(List<AddDutyDto> newDuties)
        {
            if (newDuties == null) return BadRequest(InvalidDutyErrorMessage);
            var locationIds = newDuties.SelectDistinctToList(d => d.LocationId);
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            var duty = await DutyRosterService.AddDuties(newDuties.Adapt<List<Duty>>());
            return Ok(duty.Adapt<List<DutyDto>>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditDuties)]
        public async Task<ActionResult<List<DutyDto>>> UpdateDuties(List<UpdateDutyDto> editDuties, bool overrideValidation = false)
        {
            if (editDuties == null) return BadRequest(InvalidDutyErrorMessage);
            var locationIds = await DutyRosterService.GetDutiesLocations(editDuties.SelectToList(d => d.Id));
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            var duties = await DutyRosterService.UpdateDuties(editDuties.Adapt<List<Duty>>(), overrideValidation);
            return Ok(duties.Adapt<List<DutyDto>>());
        }

        [HttpPut("updateComment")]
        [PermissionClaimAuthorize(perm: Permission.EditDuties)]
        public async Task<ActionResult> UpdateDutyComment(int dutyId, string comment)
        {
            var locationIds = await DutyRosterService.GetDutiesLocations(new List<int> {dutyId});
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            await DutyRosterService.UpdateDutyComment(dutyId, comment);
            return NoContent();
        }

        [HttpPut("moveSheriff")]
        [PermissionClaimAuthorize(perm: Permission.EditDuties)]
        public async Task<ActionResult<DutyDto>> MoveSheriffFromDutySlot(int fromDutySlotId, int toDutyId, DateTimeOffset? separationTime = null)
        {
            var duty = await DutyRosterService.GetDutyByDutySlot(fromDutySlotId);
            if (duty == null) return NotFound();
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, duty.LocationId)) return Forbid();

            duty = await DutyRosterService.MoveSheriffFromDutySlot(fromDutySlotId, toDutyId, separationTime);
            return Ok(duty.Adapt<DutyDto>());
        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: Permission.ExpireDuties)]
        public async Task<ActionResult> ExpireDuties(List<int> ids)
        {
            if (ids == null) return BadRequest(InvalidDutyErrorMessage);
            var locationIds = await DutyRosterService.GetDutiesLocations(ids);
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            await DutyRosterService.ExpireDuties(ids);
            return NoContent();
        }
    }
}
