package com.senabo.domain.disease.service;

import com.senabo.domain.disease.dto.response.DiseaseResponse;
import com.senabo.domain.disease.entity.Disease;
import com.senabo.domain.disease.repository.DiseaseRepository;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.service.ReportService;
import com.senabo.exception.message.ExceptionMessage;
import com.senabo.exception.model.UserException;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@Slf4j
@Service
@Transactional(readOnly = true)
@RequiredArgsConstructor
public class DiseaseService {
    private final DiseaseRepository diseaseRepository;
    private final MemberService memberService;

    @Transactional
    public DiseaseResponse createDisease(String email, String diseaseName) {
        Member member = memberService.findByEmail(email);
        Disease disease = diseaseRepository.save(
                new Disease(member, diseaseName));
        try {
            diseaseRepository.flush();
        } catch (DataIntegrityViolationException e) {
            throw new UserException(String.valueOf(ExceptionMessage.FAIL_SAVE_DATA));
        }
        return DiseaseResponse.from(disease);
    }

    @Transactional
    public List<Disease> getDisease(String email) {
        Member member = memberService.findByEmail(email);
        List<Disease> diseaseList = diseaseRepository.findByMemberId(member);
        return diseaseList;
    }

    @Transactional
    public List<Disease> getDiseaseWeek(Report report, Member member) {
        LocalDateTime startTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS);
        LocalDateTime endTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS).plusDays(7);
        List<Disease> diseaseList = diseaseRepository.findDiseaseWeek(member, endTime, startTime);
        return diseaseList;
    }

    @Transactional
    public void removeDisease(String email) {
        try {
            Member member = memberService.findByEmail(email);
            List<Disease> list = diseaseRepository.deleteByMemberId(member);
        } catch (DataIntegrityViolationException e) {
            throw new UserException(ExceptionMessage.FAIL_DELETE_DATA);
        }
    }

    @Transactional
    public Long getCountLastWeekDiseaseList(Member member, LocalDateTime lastStart) {
        Long count = diseaseRepository.countLastWeekData(member, lastStart);
        return count;
    }


}
