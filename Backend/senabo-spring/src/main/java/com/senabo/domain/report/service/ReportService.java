package com.senabo.domain.report.service;

import com.senabo.domain.affection.entity.Affection;
import com.senabo.domain.affection.service.AffectionService;
import com.senabo.domain.bath.entity.Bath;
import com.senabo.domain.bath.service.BathService;
import com.senabo.domain.brushingTeeth.service.BrushingTeethService;
import com.senabo.domain.communication.service.CommunicationService;
import com.senabo.domain.disease.service.DiseaseService;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.dto.response.ReportResponse;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.repository.ReportRepository;
import com.senabo.domain.stress.entity.Stress;
import com.senabo.domain.stress.entity.StressType;
import com.senabo.domain.stress.service.StressService;
import com.senabo.exception.message.ExceptionMessage;
import com.senabo.exception.model.DataException;
import com.senabo.exception.model.UserException;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.Duration;
import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.util.List;
import java.util.Optional;

@Slf4j


@Service
@Transactional(readOnly = true)
@RequiredArgsConstructor
public class ReportService {
    private final ReportRepository reportRepository;
    private final MemberService memberService;
    private final AffectionService affectionService;
    private final StressService stressService;
    private final DiseaseService diseaseService;
    private final CommunicationService communicationService;
    private final BathService bathService;
    private final BrushingTeethService brushingTeethService;

    @Transactional
    public List<Report> getReport(String email) {
        Member member = memberService.findByEmail(email);
        List<Report> reportList = reportRepository.findCompleteReport(member);
        return reportList;
    }

    @Transactional
    public Optional<Report> getReportWeek(String email, int week) {
        Member member = memberService.findByEmail(email);
        return findReportWeek(member, week);
    }

    @Transactional
    public Optional<Report> findReportWeek(Member member, int week) {
        Optional<Report> report = reportRepository.findCompleteReportWeek(member, week);
        return report;
    }

    public Report findLatestData(Member member) {
        Optional<Report> reportOptional = reportRepository.findLatestData(member);
        if (reportOptional.isEmpty()) {
            throw new DataException(ExceptionMessage.DATA_NOT_FOUND);
        }
        return reportOptional.get();
    }

    // 앱 사용 시간 저장
    @Transactional
    public ReportResponse updateTotalTime(String email, int totalTime) {
        Report report = findLatestData(memberService.findByEmail(email));
        report.updateTotalTime(totalTime);
        return ReportResponse.from(report);
    }

    public boolean check7Days(Member member) {
        // 마지막 리포트의 create datetime 가져오기
        Report lastReport = findLatestData(member);
        LocalDateTime lastCreateTime = lastReport.getCreateTime();
        LocalDateTime now = LocalDateTime.now().truncatedTo(ChronoUnit.DAYS);
        LocalDateTime lastStart = lastCreateTime.truncatedTo(ChronoUnit.DAYS);

        // 7일이 되었으면 새로운 report 생성
        Duration duration = Duration.between(now, lastStart);
        long days = duration.toDays();
        if (days == 7) return true;
        return false;
    }

    @Transactional
    public void scheduleReport(Member member) {
        try {
            // 마지막 리포트 정보 가져오기
            Report lastReport = findLatestData(member);
            LocalDateTime lastCreateTime = lastReport.getCreateTime().truncatedTo(ChronoUnit.DAYS);
            LocalDateTime now = LocalDateTime.now().truncatedTo(ChronoUnit.DAYS);
            LocalDateTime lastStart = lastCreateTime.truncatedTo(ChronoUnit.DAYS);

            // 7일 이상 경과하면 새 리포트 생성
            long daysElapsed = Duration.between(now, lastStart).toDays();
            if (daysElapsed < 7) return;

            // 각각의 스코어 계산
            int endAffectionScore = affectionService.getLatestAffectionData(member).getScore();
            int endStressScore = stressService.getLatestStressData(member).getScore();
            int poopScore = calculateStressScore(member, StressType.POOP, 168);
            int walkScore = calculateStressScore(member, StressType.WALK, 7);
            int diseaseScore = (int) Math.max(0, 100 - diseaseService.getCountLastWeekDiseaseList(member, lastStart) * 10);
            int feedScore = calculateStressScore(member, StressType.FEED, 168);
            int communicationScore = calculateCommunicationScore(member, lastStart);

            // lastReport 업데이트
            lastReport.update(endAffectionScore, endStressScore, poopScore, walkScore, feedScore, communicationScore, diseaseScore);
            ReportResponse.from(lastReport);

            // 마지막 주차인지 확인
            if (lastReport.getWeek() == 8) {
                member.complete();
                return;
            }

            // 5주 간격 목욕 -> 20
            if (lastReport.getWeek() >= 5 && stressService.getStressType(member, StressType.BATH).isEmpty() && bathService.getBath(member.getEmail()).isEmpty()) {
                stressService.saveStress(member, StressType.BATH, 20);
            }

            // 양치 최소 1회 확인 -> 10
            if (brushingTeethService.getBrushingTeethWeek(lastReport, member).isEmpty()) {
                stressService.saveStress(member, StressType.BRUSHING_TEETH, 10);
            }

            // newReport 저장
            Report newReport = reportRepository.save(new Report(member, lastReport.getWeek() + 1, endAffectionScore, endStressScore));
            reportRepository.flush();

        } catch (Exception e) {
            e.printStackTrace();
            // 예외 처리를 추가하세요.
        }
    }

    private int calculateStressScore(Member member, StressType stressType, int divisor) {
        Long stressCnt = stressService.getCountLastWeekList(member, findLatestData(member).getCreateTime().truncatedTo(ChronoUnit.DAYS), stressType);
        return (int) (1 - stressCnt / divisor) * 100;
    }

    private int calculateCommunicationScore(Member member, LocalDateTime lastStart) {
        Long communicationCnt = communicationService.getCountCommunicationWeek(member, lastStart);
        int communicationScore = communicationCnt.intValue();
        return Math.min(100, communicationScore);
    }



}
