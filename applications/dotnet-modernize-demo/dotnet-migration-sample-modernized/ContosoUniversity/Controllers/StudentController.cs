/**
 * Copyright 2025 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License
 */

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using ContosoUniversity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContosoUniversity.Controllers
{
    public class StudentController : Controller
    {
        private readonly SchoolContext db;
        private readonly ILogger<StudentController> _logger;

        public StudentController(SchoolContext db, ILogger<StudentController> logger)
        {
            this.db = db;
            _logger = logger;
        }

        // GET: Student
        public ViewResult Index()
        {
            return View(BuildIndexData(new Student()));
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("LastName, FirstMidName, EnrollmentDate", Prefix = "NewStudent")]Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException dex)
            {
                _logger.LogError(dex, "Unable to save student changes.");
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View("Index", BuildIndexData(student));
        }

        private StudentIndexData BuildIndexData(Student newStudent)
        {
            return new StudentIndexData
            {
                Students = db.Students.OrderBy(s => s.LastName).ToList(),
                EnrollmentSummary = GetEnrollmentSummary(),
                NewStudent = newStudent
            };
        }

        private List<EnrollmentDateGroup> GetEnrollmentSummary()
        {
            // Commenting out LINQ to show how to do the same thing in SQL.
            //IQueryable<EnrollmentDateGroup> = from student in db.Students
            //           group student by student.EnrollmentDate into dateGroup
            //           select new EnrollmentDateGroup()
            //           {
            //               EnrollmentDate = dateGroup.Key,
            //               StudentCount = dateGroup.Count()
            //           };

            // SQL version of the above LINQ code for PostgreSQL.
            string query = "SELECT \"EnrollmentDate\", COUNT(*) AS \"StudentCount\" "
                + "FROM \"Student\" "
                + "GROUP BY \"EnrollmentDate\"";
            IEnumerable<EnrollmentDateGroup> data = db.Database.SqlQueryRaw<EnrollmentDateGroup>(query);

            return data.ToList();
        }
    }
}
