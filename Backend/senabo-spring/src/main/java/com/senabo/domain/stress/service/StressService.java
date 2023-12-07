package com.senabo.domain.stress.service;

import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.service.ReportService;
import com.senabo.domain.stress.dto.response.StressResponse;
import com.senabo.domain.stress.entity.Stress;
import com.senabo.domain.stress.entity.StressType;
import com.senabo.domain.stress.repository.StressRepository;
import com.senabo.exception.message.ExceptionMessage;
import com.senabo.exception.model.DataException;
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
import java.util.Map;
import java.util.Optional;

@Slf4j
@Service
@Transactional(readOnly = true)
@RequiredArgsConstructor
public class StressService {
    private final StressRepository stressRepository;
    private final MemberService memberService;

    @Transactional
    public StressResponse saveStress(Member member, StressType type, int changeAmount) {
        log.info("스트레스 저장");
        int originStress = member.getStressLevel();
        int score = originStress + changeAmount;
        if (score > 100) score = 100;
        if (score < 0) score = 0;
        Stress stress = stressRepository.save(
                new Stress(member, type, changeAmount, score)
        );
        memberService.updateStress(member.getEmail(), score);
        try {
            stressRepository.flush();
        } catch (DataIntegrityViolationException e) {
            throw new UserException(String.valueOf(ExceptionMessage.FAIL_SAVE_DATA));
        }
        return StressResponse.from(stress);
    }


    public StressResponse createStress(String email, StressType type, int changeAmount) {
        Member member = memberService.findByEmail(email);
        return saveStress(member, type, changeAmount);
    }


    public List<Stress> getStress(String email) {
        Member member = memberService.findByEmail(email);
        List<Stress> stressList = stressRepository.findByMemberId(member);
        return stressList;
    }

    @Transactional
    public List<Stress> getStressWeek(Report report, Member member) {
        LocalDateTime startTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS);
        LocalDateTime endTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS).plusDays(7);
        List<Stress> stressList = stressRepository.findStressWeek(member, endTime, startTime);
        return stressList;
    }


    public Stress getLatestStressData(Member member) {
        Optional<Stress> stressOptional = stressRepository.findLatestData(member);
        if (stressOptional.isEmpty()) throw new DataException(ExceptionMessage.DATA_NOT_FOUND);
        return stressOptional.get();
    }

    @Transactional
    public List<Stress> getLastWeekList(Member member, LocalDateTime lastStart, StressType type) {
        List<Stress> list = stressRepository.findLastWeekData(member, lastStart, type);
        return list;
    }

    public Long getCountLastWeekList(Member member, LocalDateTime lastStart, StressType type) {
        Long count = stressRepository.countLastWeekData(member, lastStart, type);
        return count;
    }

    public int getEndStressScore(Member member){
        return getLatestStressData(member).getScore();
    }

    public int getScore(Member member, LocalDateTime lastStart, StressType type){
        Long stressCnt = getCountLastWeekList(member, lastStart, type);
        return  (int) (1 - stressCnt / 168) * 100;
    }

    public List<Stress> getStressType (Member member, StressType type){
        return stressRepository.findByMemberIdAndTypeOrderByCreateTimeDesc(member, type);
    }

}
