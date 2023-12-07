package com.senabo.domain.report.repository;

import com.senabo.domain.member.entity.Member;
import com.senabo.domain.report.entity.Report;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;


public interface ReportRepositoryCustom  {
    List<Report> findByMemberId(Member member);
    Optional<Report> findLatestData(Member member);
    List<Report> findCompleteReport(Member member);
    Optional<Report> findCompleteReportWeek(Member member, int week);

}