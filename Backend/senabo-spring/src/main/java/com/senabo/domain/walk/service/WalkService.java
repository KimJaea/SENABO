package com.senabo.domain.walk.service;

import com.senabo.common.ActivityType;
import com.senabo.common.message.ParsingMessageService;
import com.senabo.config.firebase.FCMService;
import com.senabo.domain.affection.service.AffectionService;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.service.ReportService;
import com.senabo.domain.stress.entity.StressType;
import com.senabo.domain.stress.service.StressService;
import com.senabo.domain.walk.dto.request.UpdateWalkRequest;
import com.senabo.domain.walk.dto.response.TodayWalkResponse;
import com.senabo.domain.walk.dto.response.WalkResponse;
import com.senabo.domain.walk.entity.Walk;
import com.senabo.domain.walk.repository.WalkRepository;
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
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@Slf4j
@Service
@Transactional(readOnly = true)
@RequiredArgsConstructor
public class WalkService {
    private final WalkRepository walkRepository;
    private final MemberService memberService;
    private final StressService stressService;
    private final AffectionService affectionService;

    @Transactional
    public WalkResponse createWalk(String email) {
        Member member = memberService.findByEmail(email);
        LocalDateTime startTime = LocalDateTime.now();
        Walk walk = walkRepository.save(
                new Walk(member, startTime, null, 0.0));
        try {
            walkRepository.flush();
        } catch (DataIntegrityViolationException e) {
            throw new UserException(String.valueOf(ExceptionMessage.FAIL_SAVE_DATA));
        }
        return WalkResponse.from(walk);
    }

    @Transactional
    public List<Walk> getWalk(String email) {
        Member member = memberService.findByEmail(email);
        List<Walk> walkList = walkRepository.findByMemberId(member);
        return walkList;
    }

    @Transactional
    public List<Walk> getWalkWeek(Report report, Member member) {
        LocalDateTime startTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS);
        LocalDateTime endTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS).plusDays(7);
        List<Walk> walkList = walkRepository.findWalkWeek(member, endTime, startTime);
        return walkList;
    }

    @Transactional
    public List<Walk> getTodayWalk(String email) {
        Member member = memberService.findByEmail(email);
        List<Walk> walkList = walkRepository.findTodayData(member, LocalDateTime.now().truncatedTo(ChronoUnit.DAYS));

        return walkList;
    }

    public TodayWalkResponse getTodayTotal(List<Walk> walkList) {
        Duration totalTime = Duration.ofSeconds(0);
        Double totalDistance = 0.0;
        for (int i = 0; i < walkList.size(); i++) {
            Walk walk = walkList.get(i);
            totalDistance += walk.getDistance();
            Duration walkDuration = Duration.between(walk.getStartTime(), walk.getEndTime());
            totalTime = totalTime.plus(walkDuration);
        }
        return TodayWalkResponse.from((int) totalTime.toMinutes(), totalDistance);
    }
    @Transactional
    public WalkResponse getWalkLatest(String email) {
        Member member = memberService.findByEmail(email);
        Optional<Walk> walkOptional = walkRepository.findLatestData(member);
        if(walkOptional.isEmpty()) throw new DataException(ExceptionMessage.DATA_NOT_FOUND);
        return WalkResponse.from(walkOptional.get());
    }

    @Transactional
    public WalkResponse updateWalk(String email, UpdateWalkRequest request) {
        Member member = memberService.findByEmail(email);
        Optional<Walk> walkOptional = walkRepository.findLatestData(member);
        if(walkOptional.isEmpty()) throw new DataException(ExceptionMessage.DATA_NOT_FOUND);
        Walk walk = walkOptional.get();
        if (walk.getEndTime() == null) {
            walk.update(LocalDateTime.now(), request.distance());
        } else {
            throw new UserException(ExceptionMessage.FAIL_UPDATE_DATA);
        }
        return WalkResponse.from(walk);
    }

    @Transactional
    public void removeWalk(String email) {
        try {
            Member member = memberService.findByEmail(email);
            List<Walk> list = walkRepository.deleteByMemberId(member);
        } catch (DataIntegrityViolationException e) {
            throw new UserException(ExceptionMessage.FAIL_DELETE_DATA);
        }
    }


    @Transactional
    public void scheduleCheckWalk(Member member) {
        int changeAffectionAmount = 0;
        int changeStressAmount = 0;

        LocalDateTime startToday = LocalDateTime.now().minus(Duration.ofDays(1)).truncatedTo(ChronoUnit.DAYS);

        List<Walk> list = walkRepository.findTodayData(member, startToday);
        double totalDistance = 0.0;
        for (Walk walk : list) {
            totalDistance += walk.getDistance();
        }

        log.info("총 산책 거리: " + totalDistance);

        if (totalDistance < 5.0) {
            changeAffectionAmount = -5;
            changeStressAmount = 10;
        } else {
            changeAffectionAmount = 5;
            changeStressAmount = -50;
        }
        stressService.saveStress(member, StressType.WALK, changeStressAmount);
        affectionService.saveAffection(member, ActivityType.WALK, changeAffectionAmount);
    }


    public Optional<Walk> findLatestData(Member member){
        return walkRepository.findLatestData(member);
    }
}
