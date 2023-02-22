import { assignmentCardWeekInfoType } from '@/types/DutyRoster';
import { shiftRangeInfoType, sheriffAvailabilityInfoType, distributeTeamMemberInfoType } from '@/types/ShiftSchedule';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class AssignmentScheduleInformation extends VuexModule {

  public assignmentRangeInfo = {} as shiftRangeInfoType;
  public sheriffsAvailabilityInfo = [] as sheriffAvailabilityInfoType[];
  public selectedShifts = [] as string[];
  public teamMemberList = [] as distributeTeamMemberInfoType[];

  public dutyShiftAssignmentsWeek: assignmentCardWeekInfoType[] = [];

  @Mutation
  public setAssignmentRangeInfo(assignmentRangeInfo): void {   
    this.assignmentRangeInfo = assignmentRangeInfo
  }

  @Action
  public UpdateAssignmentRangeInfo(newAssignmentRangeInfo): void {
    this.context.commit('setAssignmentRangeInfo', newAssignmentRangeInfo)
  }

  @Mutation
  public setSheriffsAvailabilityInfo(sheriffsAvailabilityInfo): void {   
    this.sheriffsAvailabilityInfo = sheriffsAvailabilityInfo
  }

  @Action
  public UpdateSheriffsAvailabilityInfo(newSheriffsAvailabilityInfo): void {
    this.context.commit('setSheriffsAvailabilityInfo', newSheriffsAvailabilityInfo)
  }

  @Mutation
  public setSelectedShifts(selectedShifts): void {   
    this.selectedShifts = selectedShifts
  }

  @Action
  public UpdateSelectedShifts(newSelectedShifts): void {
    this.context.commit('setSelectedShifts', newSelectedShifts)
  }

  @Mutation
  public setTeamMemberList(teamMemberList): void {   
    this.teamMemberList = teamMemberList
  }

  @Action
  public UpdateTeamMemberList(newTeamMemberList): void {
    this.context.commit('setTeamMemberList', newTeamMemberList)
  }

  @Mutation
  public setDutyShiftAssignmentsWeek(dutyShiftAssignmentsWeek: assignmentCardWeekInfoType[]): void {   
    this.dutyShiftAssignmentsWeek = dutyShiftAssignmentsWeek
  }

  @Action
  public UpdateDutyShiftAssignmentsWeek(newDutyShiftAssignmentsWeek: assignmentCardWeekInfoType[]): void {
    this.context.commit('setDutyShiftAssignmentsWeek', newDutyShiftAssignmentsWeek)
  }

}

export default AssignmentScheduleInformation