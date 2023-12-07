package com.senabo.domain.communication.service;

import com.senabo.common.ActivityType;
import com.senabo.domain.affection.service.AffectionService;
import com.senabo.domain.communication.dto.response.CommunicationResponse;
import com.senabo.domain.communication.entity.Communication;
import com.senabo.domain.communication.repository.CommunicationRepository;
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
public class CommunicationService {
    private final CommunicationRepository communicationRepository;
    private final MemberService memberService;

    @Transactional
    public CommunicationResponse createCommunication(Member member, ActivityType type) {
        Communication communication = communicationRepository.save(
                new Communication(member, type));
        try {
            communicationRepository.flush();
        } catch (DataIntegrityViolationException e) {
            throw new UserException(String.valueOf(ExceptionMessage.FAIL_SAVE_DATA));
        }

        return CommunicationResponse.from(communication);
    }


    @Transactional
    public List<Communication> getCommunication(String email) {
        Member member = memberService.findByEmail(email);
        List<Communication> communicationList = communicationRepository.findByMemberId(member);
        return communicationList;
    }

    @Transactional
    public List<Communication> getCommunicationWeek(Report report, Member member) {
        LocalDateTime startTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS);
        LocalDateTime endTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS).plusDays(7);
        List<Communication> communicationList = communicationRepository.findCommunicationWeek(member, endTime, startTime);
        return communicationList;
    }

    @Transactional
    public void removeCommunication(String email) {
        try {
            Member member = memberService.findByEmail(email);
            List<Communication> list = communicationRepository.deleteByMemberId(member);
        } catch (DataIntegrityViolationException e) {
            throw new UserException(ExceptionMessage.FAIL_DELETE_DATA);
        }
    }

    public Long getCountCommunicationWeek(Member member, LocalDateTime lastStart){
        return communicationRepository.countCommunicationWeek(member, lastStart);
    }
}
