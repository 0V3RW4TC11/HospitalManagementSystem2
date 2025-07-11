﻿namespace HospitalManagementSystem2.Models.Entities;

public class Appointment : Entity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; }
    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; }
    public DateTime Date { get; set; }
}