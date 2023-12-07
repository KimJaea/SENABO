package com.senabo.domain.report.repository;

import com.senabo.domain.member.entity.Member;
import com.senabo.domain.report.entity.Report;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

@Repository
public interface ReportRepository extends JpaRepository<Report, String>, ReportRepositoryCustom {
    List<Report> deleteByMemberId(Member member);
}